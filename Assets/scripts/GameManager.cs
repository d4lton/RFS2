﻿using System.Collections;
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

	public static GameManager instance;

	void Awake() {
		instance = this;
	}

	void Start() {
		setState((int)GameState.RUNNING);
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
			GameObject asteroid = Instantiate(asteroidPrefab);
			yield return new WaitForSeconds(Random.Range(asteroidSpawnRateMin, asteroifSpawnRateMax));
		}
	}

	void onPlayerDied() {
		setState((int)GameState.PLAYER_DIED);
	}

	protected override void onStateChange() {
		switch ((GameState)state) {
		case GameState.ENDED:
			// show startPage
			break;
		case GameState.RUNNING:
			initializeGame();
			break;
		case GameState.PLAYER_DIED:
			// show gameOverPage
			StopCoroutine("spawnAsteroids");
			break;
		}
	}

	private void initializeGame() {
		// spawn launchpads
		for (int i = 0; i < 3; i++) {
			GameObject pad = Instantiate(padPrefab);
			pad.transform.position = new Vector3(xPadOffset * (i - 1), yPad, 0);
		}
		// start asteroids
		StartCoroutine("spawnAsteroids");
	}

}
