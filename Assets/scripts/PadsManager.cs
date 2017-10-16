using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadsManager : MonoBehaviour {

	public delegate void PadsManagerDelegate();
	public static event PadsManagerDelegate onPadsCreated;
	public static event PadsManagerDelegate onPadsDestroyed;

	public GameObject padPrefab;
	public int padCount = 3;
	public float padCreateInterval = 1.0f;
	public float yPad = -4.0f;
	public float xPadOffset = -2.0f;

	public static PadsManager instance;

	int padsLeft = 0;
	bool gameRunning = false;

	void Awake() {
		instance = this;
	}

	void Start() {
	}
	
	void Update() {
		if (gameRunning) {
			// do the stuff and things
		}
	}

	void OnEnable() {
		GameManager.onGameStarted += onGameStarted;
		GameManager.onGameRunning += onGameRunning;
		GameManager.onGameEnded += onGameEnded;
	}

	void OnDisable() {
		GameManager.onGameStarted -= onGameStarted;
		GameManager.onGameRunning -= onGameRunning;
		GameManager.onGameEnded -= onGameEnded;
	}

	void onGameStarted() {
		Debug.Log("PadsManager onGameStarted");
		createPads();
	}

	void onGameRunning() {
		Debug.Log("PadsManager onGameRunning");
		gameRunning = true;
	}

	void onGameEnded() {
		Debug.Log("PadsManager onGameEnded");
		gameRunning = false;
	}

	void onPadDestroyed() {
		Debug.Log("onPadDestroyed");
		padsLeft--;
		if (padsLeft <= 0) {
			if (onPadsDestroyed != null) {
				onPadsDestroyed();
			}
		}
	}

	public void createPads() {
		StartCoroutine("spawnPads");
	}

	IEnumerator spawnPads() {
		for (int i = 0; i < padCount; i++) {
			Debug.Log("creating pad");
			createPad(i);
			yield return new WaitForSeconds(padCreateInterval);
		}
		padsLeft = padCount;
		if (onPadsCreated != null) {
			onPadsCreated();
		}
	}

	private void createPad(int i) {
		GameObject pad = Instantiate(padPrefab);
		PadManager padManager = pad.GetComponent<PadManager>();
		padManager.onPadDestroyed += onPadDestroyed;
		pad.transform.position = new Vector3(xPadOffset * (i - (padCount / 2)), yPad, 0);
	}

}
