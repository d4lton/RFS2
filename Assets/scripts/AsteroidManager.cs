using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidManager : MonoBehaviour {

	Rigidbody2D rigidBody;

	public float yStart = 7.0f;
	public float xStartOffset = 3.0f;
	public float xForce = 80.0f;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.transform.position = new Vector3((float)Random.Range(-xStartOffset, xStartOffset), yStart, 0f);
		// TODO: the x position of the asteroid should be used to temper the force, so asteroids don't go flying off-screen
		rigidBody.AddForce(Vector2.right * (float)Random.Range(-xForce, xForce), ForceMode2D.Force);
	}
	
	void Update() {
	}

	void OnEnable() {
		GameManager.onGameEnded += onGameEnded;
	}

	void OnDisable() {
		GameManager.onGameEnded -= onGameEnded;
	}

	void onGameEnded() {
		Debug.Log("AsteroidManager noticed that the game ended, fascinating.");
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Ground") {
			Destroy(gameObject);
		}
	}

}
