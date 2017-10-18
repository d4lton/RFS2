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

	public static PadsManager instance;

	void Awake() {
		instance = this;
	}

	void OnEnable() {
		GameManager.onGameStarted += onGameStarted;
	}

	void OnDisable() {
		GameManager.onGameStarted -= onGameStarted;
	}

	void onGameStarted() {
		createPads();
	}

	void onPadDestroyed() {
		GameObject[] pads = GameObject.FindGameObjectsWithTag("Pad");
		int padsLeft = pads.Length - 1; // really 1 less, since the one sending the event still counts
		if (padsLeft <= 0) {
			if (onPadsDestroyed != null) {
				onPadsDestroyed();
			}
		}
	}

	private Pad getBestPadToLaunch(Vector3 mousePosition) {
		GameObject[] pads = GameObject.FindGameObjectsWithTag("Pad");
		Pad bestPad = null;
		float minDistance = Mathf.Infinity;
		// find pad with rocket, closest horizontally to the mouse click
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

	private void createPads() {
		StartCoroutine("spawnPads");
	}

	private void createPad(int i) {
		GameObject pad = makeNewPad();
		pad.transform.position = new Vector3(xPadOffset * (i - (padCount / 2)), yPad, 0);
	}

	private GameObject makeNewPad() {
		GameObject pad = Instantiate(padPrefab);
		Pad padScript = pad.GetComponent<Pad>();
		padScript.onPadDestroyed += onPadDestroyed;
		return pad;
	}

	IEnumerator spawnPads() {
		for (int i = 0; i < padCount; i++) {
			createPad(i);
			yield return new WaitForSeconds(padCreateInterval);
		}
		if (onPadsCreated != null) {
			onPadsCreated();
		}
	}

	public void launchRocket(Vector3 target) {
		// find the best pad to launch from, if any
		Pad bestPad = getBestPadToLaunch(target);
		if (bestPad != null) {
			bestPad.fire(target);
		} else {
			// TODO: ruh roh, no can fire! make blerp sound!
		}
	}

	public void repairPad(GameObject brokenPad) {
		GameObject pad = makeNewPad();
		pad.transform.position = brokenPad.transform.position;
		BrokenPad brokenPadScript = brokenPad.GetComponent<BrokenPad>();
		brokenPadScript.die();
	}

}
