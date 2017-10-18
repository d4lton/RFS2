using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : StateMachineBehavior {
	
	public delegate void ScoreDelegate(int value);
	public static event ScoreDelegate onScored;

	public enum PadState {
		IDLE = 0,
		LOADING,
		LOADED
	};

	public delegate void PadDelegate();
	public event PadDelegate onPadDestroyed;

	public GameObject rocketPrefab;
	public GameObject explosionPrefab;
	public GameObject brokenPadPrefab;
	public float makeRocketDelay = 3.0f;

	GameObject rocket;

	void Start() {
		setState((int)PadState.IDLE);
	}

	void OnEnable() {
		GameManager.onGameEnded += onGameEnded;
	}

	void OnDisable() {
		GameManager.onGameEnded -= onGameEnded;
	}

	void onGameEnded() {
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			if (rocket != null) { // there's a rocket on the pad, kill it too
				rocket.GetComponent<Rocket>().destroy();
				if (onScored != null) {
					onScored(-100);
				}
			} else {
				if (onScored != null) {
					onScored(-50);
				}
			}
			explode();
		}
	}

	void explode() {
		GameManager.instance.playClip("pad");

		// create an explosion over the pad
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.GetComponent<Explosion>().disableColliders();
		explosion.transform.position = transform.position;

		// create a broken pad over the pad

		GameObject brokenPad = Instantiate(brokenPadPrefab) as GameObject;
		brokenPad.transform.position = transform.position;

		//source.PlayOneShot(explosionClip);

		// remove the pad
		if (onPadDestroyed != null) {
			onPadDestroyed();
		}
		Destroy(gameObject);
	}

	void onRocketDestroyed() {
		setState((int)PadState.IDLE);
	}

	protected override void onStateChange() {
		switch ((PadState)state) {
		case PadState.IDLE:
			rocket = null;
			setState((int)PadState.LOADING);
			break;
		case PadState.LOADING:
			StartCoroutine("spawnRocket");
			break;
		case PadState.LOADED:
			StopCoroutine("spawnRocket"); // this probably already stops by itself
			break;
		}
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

	IEnumerator spawnRocket() {
		yield return new WaitForSeconds(makeRocketDelay);
		createRocket();
	}

	public bool hasRocket() {
		return (state == (int)PadState.LOADED);
	}

	public void fire(Vector3 target) {
		rocket.GetComponent<Rocket>().setTarget(target);
		setState((int)PadState.IDLE);
	}

}
