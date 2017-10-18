using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPad : MonoBehaviour {

	public GameObject explosionPrefab;

	void OnEnable() {
		GameManager.onGameEnded += onGameEnded;
	}

	void OnDisable() {
		GameManager.onGameEnded -= onGameEnded;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			explode();
		}
	}

	void onGameEnded() {
		die();
	}

	void explode() {
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.GetComponent<Explosion>().disableColliders();
		explosion.transform.position = transform.position;
		die();
	}

	public void die() {
		Destroy(gameObject);
	}

}
