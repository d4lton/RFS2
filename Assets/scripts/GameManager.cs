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
	public int initialScore = 500;

	public static GameManager instance;

	int score;

	void Awake() {
		instance = this;
	}

	void Start() {
		setState((int)GameState.ENDED);
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
		setState((int)GameState.PLAYER_DIED);
	}

	void onScored(int value) {
		addToScore(value);
		if (score < 0) {
			setState((int)GameState.PLAYER_DIED);
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

	private void endGame() {
		// stop spawning those asteroids
		StopCoroutine("spawnAsteroids");
		// let everyone know the game has ended
		if (onGameEnded != null) {
			onGameEnded();
		}
		// TODO: check for high score, maybe make some more bleepings, store high score
	}

	private void startGame() {
		resetScore();
		// let everyone know the game is starting
		if (onGameStarted != null) {
			onGameStarted();
		}
	}

	private void runGame() {
		// start spawning the asteroids
		StartCoroutine("spawnAsteroids");
		// let everyone know the game is running
		if (onGameRunning != null) {
			onGameRunning();
		}
	}

	private void showPageForState() {
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

	private void resetScore() {
		setScore(initialScore);
	}

	private void addToScore(int amount) {
		setScore(score + amount);
	}

	private void setScore(int newScore) {
		score = newScore;
		// TODO: check for high score, make some kinda bleep
		if (Score.instance != null) {
			Score.instance.setScore(score);
		}
	}

	IEnumerator spawnAsteroids() {
		while (true) {
			Instantiate(asteroidPrefab);
			yield return new WaitForSeconds(Random.Range(asteroidSpawnRateMin, asteroidSpawnRateMax));
		}
	}

	public void onPlayClicked() {
		setState((int)GameState.STARTING);
	}

	public void onRestartClicked() {
		setState((int)GameState.ENDED);
	}

}
