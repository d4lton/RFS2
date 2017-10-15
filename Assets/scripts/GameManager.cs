using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : StateMachineBehavior {

	public enum GameState {
		ENDED = 0,
		RUNNING,
		PLAYER_DIED
	};

	public delegate void GameManagerDelegate();
	public static event GameManagerDelegate onGameStarted;
	public static event GameManagerDelegate onGameEnded;

	public GameObject startPage;
	public GameObject gameOverPage;
	public GameObject asteroidPrefab;
	public GameObject padPrefab;
	public int asteroidSpawnRateMin = 2;
	public int asteroifSpawnRateMax = 4;
	public float yPad = -4.0f;
	public float xPadOffset = -2.0f;
	public int padCount = 3;

	public static GameManager instance;

	private int padsLeft = 0;

	void Awake() {
		instance = this;
	}

	void Start() {
		setState((int)GameState.ENDED);
	}

	void OnEnable() {
		PadManager.onPadDestroyed += onPadDestroyed;
	}

	void OnDisable() {
		PadManager.onPadDestroyed -= onPadDestroyed;
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
			yield return new WaitForSeconds(Random.Range(asteroidSpawnRateMin, asteroifSpawnRateMax));
		}
	}

	void onPlayerDied() {
		setState((int)GameState.PLAYER_DIED);
	}

	void onPadDestroyed() {
		Debug.Log("onPadDestroyed");
		padsLeft--;
		if (padsLeft <= 0) {
			setState((int)GameState.PLAYER_DIED);
		}
	}

	public void onPlayClicked() {
		setState((int)GameState.RUNNING);
	}

	public void onRestartClicked() {
		setState((int)GameState.ENDED);
	}

	protected override void onStateChange() {
		switch ((GameState)state) {
		case GameState.ENDED:
			Debug.Log("ENDED");
			startPage.SetActive(true);
			gameOverPage.SetActive(false);
			break;
		case GameState.RUNNING:
			Debug.Log("RUNNING");
			startPage.SetActive(false);
			gameOverPage.SetActive(false);
			startGame();
			break;
		case GameState.PLAYER_DIED:
			Debug.Log("PLAYER_DIED");
			endGame();
			startPage.SetActive(false);
			gameOverPage.SetActive(true);
			break;
		}
	}

	private void endGame() {
		StopCoroutine("spawnAsteroids");
		// remove any in-play asteroids and such
	}

	private void startGame() {

		padsLeft = padCount;

		// spawn launchpads
		for (int i = 0; i < padsLeft; i++) {
			GameObject pad = Instantiate(padPrefab);
			pad.transform.position = new Vector3(xPadOffset * (i - (padCount / 2)), yPad, 0);
		}

		// start asteroids
		StartCoroutine("spawnAsteroids");
	
	}

}
