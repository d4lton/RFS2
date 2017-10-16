using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : StateMachineBehavior {

	public enum PadState {
		IDLE = 0,
		LOADING,
		LOADED
	};

	public delegate void PadDelegate();
	public event PadDelegate onPadDestroyed;

	public GameObject rocketPrefab;
	public float makeRocketDelay = 3.0f;

	GameObject rocket;

	void Start() {
		setState((int)PadState.IDLE);
	}

	void Update() {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			if (onPadDestroyed != null) {
				onPadDestroyed();
			}
			if (rocket != null) { // there's a rocket on the pad, kill it too
				rocket.GetComponent<Rocket>().destroy();
			}
			Destroy(gameObject);
		}
	}

	void onRocketDestroyed() {
		Debug.Log("ROCKET DESTROYED, BUT WE'RE SAVED!");
		setState((int)PadState.IDLE);
	}

	protected override void onStateChange() {
		switch ((PadState)state) {
		case PadState.IDLE:
			Debug.Log("IDLE");
			rocket = null;
			setState((int)PadState.LOADING);
			break;
		case PadState.LOADING:
			Debug.Log("LOADING");
			StartCoroutine("spawnRocket");
			break;
		case PadState.LOADED:
			Debug.Log("LOADED");
			StopCoroutine("spawnRocket"); // this probably already stops by itself
			break;
		}
	}

	IEnumerator spawnRocket() {
		yield return new WaitForSeconds(makeRocketDelay);
		createRocket();
	}

	void createRocket() {
		// create the rocket instance
		rocket = Instantiate(rocketPrefab) as GameObject;
		rocket.transform.position = transform.position;
		// listen for its destroyed event
		Rocket rocketScript = rocket.GetComponent<Rocket>();
		rocketScript.onRocketDestroyed += onRocketDestroyed;
		// switch to LOADED
		setState((int)PadState.LOADED);
	}

	public bool hasRocket() {
		return (state == (int)PadState.LOADED);
	}

	public void fire(Vector3 target) {
		rocket.GetComponent<Rocket>().setTarget(target);
		setState((int)PadState.IDLE);
	}

}
