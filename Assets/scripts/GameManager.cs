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
	public GameObject gameOverPage;
	public GameObject asteroidPrefab;
	public GameObject padPrefab;
	public int asteroidSpawnRateMin = 2;
	public int asteroidSpawnRateMax = 4;

	public static GameManager instance;

	void Awake() {
		instance = this;
	}

	void Start() {
		setState((int)GameState.ENDED);
	}

	void OnEnable() {
		PadsManager.onPadsCreated += onPadsCreated;
		PadsManager.onPadsDestroyed += onPadsDestroyed;
	}

	void OnDisable() {
		PadsManager.onPadsCreated -= onPadsCreated;
		PadsManager.onPadsDestroyed -= onPadsDestroyed;
	}
	
	void Update() {
		switch ((GameState)state) {
		case GameState.RUNNING:
			// handle time-based tasks like spawning asteroids and what-not
			break;
		}
	}

	IEnumerator spawnAsteroids() {
		while (true) {
			Instantiate(asteroidPrefab);
			yield return new WaitForSeconds(Random.Range(asteroidSpawnRateMin, asteroidSpawnRateMax));
		}
	}

	void onPadsCreated() {
		setState((int)GameState.RUNNING);
	}

	void onPadsDestroyed() {
		setState((int)GameState.PLAYER_DIED);
	}

	public void onPlayClicked() {
		setState((int)GameState.STARTING);
	}

	public void onRestartClicked() {
		setState((int)GameState.ENDED);
	}

	protected override void onStateChange() {
		switch ((GameState)state) {
		case GameState.ENDED:
			startPage.SetActive(true);
			gameOverPage.SetActive(false);
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
	}

	private void endGame() {
		// show the Game Over page
		startPage.SetActive(false);
		gameOverPage.SetActive(true);
		// stop spawning those asteroids
		StopCoroutine("spawnAsteroids");
		// let everyone know the game has ended
		if (onGameEnded != null) {
			onGameEnded();
		}
	}

	private void startGame() {
		// show the Start Game page
		startPage.SetActive(false);
		gameOverPage.SetActive(false);
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

}
