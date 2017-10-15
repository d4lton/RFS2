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
		rigidBody.AddForce(Vector2.right * (float)Random.Range(-xForce, xForce), ForceMode2D.Force);
	}
	
	void Update() {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Ground") {
			Destroy(gameObject);
		}
	}
}
