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

	void Awake() {
		instance = this;
	}

	void Start() {
	}
	
	void Update() {
	}

	void onEnable() {
	}

	void onDisable() {
	}

	void onPadDestroyed() {
		Debug.Log("onPadDestroyed");
		padsLeft--;
		if (padsLeft <= 0) {
			onPadsDestroyed();
		}
	}

	public void createPads() {
		// create pads, when done, call onPadsCreated()
		StartCoroutine("spawnPads");
	}

	IEnumerator spawnPads() {
		for (int i = 0; i < padCount; i++) {
			Debug.Log("creating pad");
			createPad(i);
			yield return new WaitForSeconds(padCreateInterval);
		}
		padsLeft = padCount;
		onPadsCreated();
	}

	private void createPad(int i) {
		GameObject pad = Instantiate(padPrefab);
		PadManager padManager = pad.GetComponent<PadManager>();
		padManager.onPadDestroyed += onPadDestroyed;
		pad.transform.position = new Vector3(xPadOffset * (i - (padCount / 2)), yPad, 0);
	}

}
