using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float lifetime = 0.5f;

	void Start() {
		StartCoroutine("awaitDeath");
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			if (tag == "Rocket") {
				Debug.Log("ASTEROID HIT BLASTWAVE, SCORE!");
			}
		}
	}

	IEnumerator awaitDeath() {
		yield return new WaitForSeconds(lifetime);
		die();
	}

	void die() {
		Destroy(gameObject);
	}

}