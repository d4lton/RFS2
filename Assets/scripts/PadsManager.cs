using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadsManager : MonoBehaviour {

	public delegate void PadsManagerDelegate();
	public static event PadsManagerDelegate onPadsCreated;
	public static event PadsManagerDelegate onPadsDestroyed;

	public GameObject padPrefab;
	public GameObject rocketPrefab;
	public int padCount = 3;
	public float padCreateInterval = 1.0f;
	public float yPad = -4.0f;
	public float xPadOffset = -2.0f;

	int padsLeft = 0;
	bool gameRunning = false;

	void Awake() {
	}

	void Start() {
	}
	
	void Update() {
		if (gameRunning) {
			// do the stuff and things
			if (Input.GetMouseButtonDown(0)) {
				// get mouse position in world coordinates, clear out z
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePosition.z = 0f;

				// find the best pad to launch from, if any
				Pad bestPad = getBestPadToLaunch(mousePosition);
				if (bestPad != null) {
					bestPad.fire(mousePosition);
				} else {
					// TODO: ruh roh, no can fire!
				}

			}
		}
	}

	Pad getBestPadToLaunch(Vector3 mousePosition) {
		GameObject[] pads = GameObject.FindGameObjectsWithTag("Pad");
		Pad bestPad = null;
		float minDistance = Mathf.Infinity;
		// find pad with rocket, closest vertically to the mouse click
		for (int i = 0; i < pads.Length; i++) {
			Pad padScript = pads[i].GetComponent<Pad>();
			if (padScript.hasRocket()) {
				float distance = Mathf.Abs(mousePosition.x - pads[i].transform.position.x);
				if (distance < minDistance) {
					minDistance = distance;
					bestPad = padScript;
				}
			}
		}
		return bestPad;
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
		createPads();
	}

	void onGameRunning() {
		gameRunning = true;
	}

	void onGameEnded() {
		gameRunning = false;
	}

	void onPadDestroyed() {
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
		Pad padScript = pad.GetComponent<Pad>();
		padScript.onPadDestroyed += onPadDestroyed;
		pad.transform.position = new Vector3(xPadOffset * (i - (padCount / 2)), yPad, 0);
	}

}
