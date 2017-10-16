using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour {

	public float force = 10f;
	public float initialVelocity = 9.8f;
	public float maxY = 5.0f;
	public float rotationRate = 1.5f;

	Rigidbody2D rigidBody;

	float targetAngle;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.velocity = new Vector2(0, initialVelocity);
	}
	
	void Update() {
		if (rigidBody.position.y > maxY) {
			Destroy(gameObject);
			return;
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * rotationRate);

		rigidBody.AddForce(rigidBody.transform.up * Time.deltaTime * force);
	
	}

	public void setTarget(Vector3 target) {
		Vector3 direction = transform.position - target;
		direction.Normalize();
		targetAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90.0f;
		//transform.rotation = Quaternion.Euler(0, 0, targetAngle);
	}

}
