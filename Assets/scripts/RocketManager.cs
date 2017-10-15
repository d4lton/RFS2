using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketManager : MonoBehaviour {

	public float force = 10f;
	public float initialVelocity = 9.8f;

	Rigidbody2D rigidBody;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.velocity = new Vector2(0, initialVelocity);
	}
	
	void Update() {
		rigidBody.AddForce(Vector2.up * force * Time.deltaTime);
	}
}
