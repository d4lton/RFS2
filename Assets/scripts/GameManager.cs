using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : StateMachineBehavior {

	public enum GameState {
		ENDED = 0,
		STARTING,
		RUNNING,
		PLAYER_DIED
	};

	public delegate void GameManagerDelegate();
	public static event GameManagerDelegate onGameStarted;
	public static event GameManagerDelegate onGameRunning;
	public static event GameManagerDelegate onGameEnded;

	public GameObject startPage;
	public GameObject playingPage;
	public GameObject gameOverPage;
	public GameObject asteroidPrefab;
	public GameObject padPrefab;
	public int asteroidSpawnRateMin = 2;
	public int asteroidSpawnRateMax = 4;
	public int asteroidSpawnCount = 1;
	public int initialScore = 500;
	public float difficultyInterval = 20.0f;
	public int maxDifficulty = 9; // 10 levels, 0 through 9
	public AudioClip difficultyIncreaseClip;
	public AudioClip explosionClip;
	public AudioClip padCrushedClip;

	public static GameManager instance;

	int score;
	float difficultyTime = 0f;
	int difficultyLevel = 0;
	AudioSource source;

	void Awake() {
		instance = this;
		source = GetComponent<AudioSource>();
	}

	void Start() {
		setState((int)GameState.ENDED);
	}

	void Update() {
		switch ((GameState)state) {
		case GameState.RUNNING:
			handleDifficultyIncrease();
			handlePrimaryMouseClick();
			break;
		}
	}

	void OnEnable() {
		PadsManager.onPadsCreated += onPadsCreated;
		PadsManager.onPadsDestroyed += onPadsDestroyed;
		Rocket.onScored += onScored;
		Explosion.onScored += onScored;
		Ground.onScored += onScored;
		Pad.onScored += onScored;
	}

	void OnDisable() {
		PadsManager.onPadsCreated -= onPadsCreated;
		PadsManager.onPadsDestroyed -= onPadsDestroyed;
		Rocket.onScored -= onScored;
		Explosion.onScored -= onScored;
		Ground.onScored -= onScored;
		Pad.onScored -= onScored;
	}

	void onPadsCreated() {
		setState((int)GameState.RUNNING);
	}

	void onPadsDestroyed() {
		if (state == (int)GameState.RUNNING) {
			Debug.Log("PLAYER DIED BECAUSE ALL PADS ARE DESTROYED");
			setState((int)GameState.PLAYER_DIED);
		}
	}

	void onScored(int value) {
		if (state == (int)GameState.RUNNING) {
			addToScore(value);
			if (score < 0) {
				Debug.Log("PLAYER DIED BECAUSE THERE'S NO MONEY LEFT");
				setState((int)GameState.PLAYER_DIED);
			}
		}
	}

	protected override void onStateChange() {
		switch ((GameState)state) {
		case GameState.ENDED:
			// you can't do that... yet.
			break;
		case GameState.STARTING:
			startGame();
			break;
		case GameState.RUNNING:
			runGame();
			break;
		case GameState.PLAYER_DIED:
			endGame();
			break;
		}
		showPageForState();
	}

	void endGame() {
		// stop spawning those asteroids
		StopCoroutine("spawnAsteroids");
		// let everyone know the game has ended
		if (onGameEnded != null) {
			onGameEnded();
		}
		// TODO: check for high score, maybe make some more bleepings, store high score
	}

	void startGame() {
		resetScore();
		resetLevel();
		difficultyTime = 0f;
		difficultyLevel = 0;
		// let everyone know the game is starting
		if (onGameStarted != null) {
			onGameStarted();
		}
	}

	void runGame() {
		// start spawning the asteroids
		StartCoroutine("spawnAsteroids");
		// let everyone know the game is running
		if (onGameRunning != null) {
			onGameRunning();
		}
	}

	void showPageForState() {
		// hide all pages
		startPage.SetActive(false);
		playingPage.SetActive(false);
		gameOverPage.SetActive(false);
		// now show the one appropriate for this state
		switch ((GameState)state) {
		case GameState.ENDED:
			startPage.SetActive(true);
			break;
		case GameState.RUNNING:
			playingPage.SetActive(true);
			break;
		case GameState.PLAYER_DIED:
			gameOverPage.SetActive(true);
			break;
		}
	}

	void resetScore() {
		setScore(initialScore);
	}

	void addToScore(int amount) {
		setScore(score + amount);
	}

	void setScore(int newScore) {
		score = newScore;
		// TODO: check for high score, make some kinda bleep
		if (Score.instance != null) {
			Score.instance.setScore(score);
		}
	}

	void resetLevel() {
		setLevel(0);
	}

	void setLevel(int level) {
		if (Level.instance != null) {
			Level.instance.setLevel(level);
		}
	}

	void handleDifficultyIncrease() {
		difficultyTime += Time.deltaTime;
		if (difficultyTime > difficultyInterval) {
			if (difficultyLevel < maxDifficulty) {
				source.PlayOneShot(difficultyIncreaseClip);
				difficultyLevel++;
				setLevel(difficultyLevel);
			} else {
				Debug.Log("AT MAX DIFFICULTY");
			}
			difficultyTime = 0f;
		}
	}

	void handlePrimaryMouseClick() {
		if (Input.GetMouseButtonDown(0)) {

			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0f;

			// determine if user clicked on a game object, or just in the play field
			Vector2 origin = new Vector2(mousePosition.x, mousePosition.y);
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 0f);
			if (hit) {
				switch (hit.transform.tag) {
				case "BrokenPad":
					repairPad(hit.transform.gameObject);
					break;
				default:
					launchRocket(mousePosition);
					break;
				}
				//Debug.Log("HIT " + hit.transform.gameObject.tag);
			} else {
				launchRocket(mousePosition);
			}

		}
	}

	void repairPad(GameObject pad) {
		if (score >= 1000) {
			addToScore(-1000);
			PadsManager.instance.repairPad(pad);
		} else {
			// hold on there fella! y'aint gots enough cabbage for that!
			Debug.Log("NEED MORE MONEY");
		}
	}

	void launchRocket(Vector3 target) {
		PadsManager.instance.launchRocket(target);
	}

	IEnumerator spawnAsteroids() {
		while (true) {
			int min = asteroidSpawnRateMin;
			int max = asteroidSpawnRateMax;
			if (difficultyLevel > 4) {
				//asteroidSpawnCount = 2; // this seems too much
				max -= 1;
			}
			for (int i = 0; i < asteroidSpawnCount; i++) {
				Instantiate(asteroidPrefab);
			}
			yield return new WaitForSeconds(Random.Range(min, max));
		}
	}

	public void onPlayClicked() {
		setState((int)GameState.STARTING);
	}

	public void onRestartClicked() {
		setState((int)GameState.ENDED);
	}

	public int getDifficultyLevel() {
		return difficultyLevel;
	}

	public void playClip(string name) {
		switch (name) {
		case "explosion":
			source.PlayOneShot(explosionClip);
			break;
		case "pad":
			source.PlayOneShot(padCrushedClip);
			break;
		default:
			Debug.Log("UNKNOWN SOUND NAME: " + name);
			break;
		}
	}

}
