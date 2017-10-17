using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	
	public delegate void ScoreDelegate(int value);
	public static event ScoreDelegate onScored;

	public float lifetime = 0.5f;

	void Start() {
		StartCoroutine("awaitDeath");
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			if (tag == "Rocket") {
				if (onScored != null) {
					onScored(100);
				}
			}
		}
	}

	private void die() {
		Destroy(gameObject);
	}

	IEnumerator awaitDeath() {
		yield return new WaitForSeconds(lifetime);
		die();
	}

	public void disableColliders() {
		foreach(Collider collider in GetComponents<Collider>()) {
			collider.enabled = false;
		}
	}

}