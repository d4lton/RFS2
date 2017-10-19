using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : StateMachineBehavior {

	public enum RocketState {
		IDLE = 0,
		FIRING
	};

	public delegate void RocketDelegate();
	public event RocketDelegate onRocketDestroyed;

	public delegate void ScoreDelegate(int value);
	public static event ScoreDelegate onScored;

	public GameObject explosionPrefab;
	public float force = 10f;
	public float initialVelocity = 9.8f;
	public float maxY = 5.0f;
	public float rotationRate = 1.5f;
	public float triggerDistance = 1.0f;

	Rigidbody2D rigidBody;

	float targetAngle;
	Vector2 targetPosition;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();
		setState((int)RocketState.IDLE);
	}
	
	void Update() {
		switch ((RocketState)state) {
		case RocketState.IDLE:
			break;
		case RocketState.FIRING:
			// if we've gone too far, time to go bye-bye
			if (rigidBody.position.y > maxY || reachedTarget()) {
				explode();
				return;
			}
			// rotate towards the target
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * rotationRate);
			// accelerate towards the target
			rigidBody.AddForce(rigidBody.transform.up * Time.deltaTime * force);
			break;
		}
	}

	void OnEnable() {
		GameManager.onGameEnded += onGameEnded;
	}

	void OnDisable() {
		GameManager.onGameEnded -= onGameEnded;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			onRocketDestroyed();
			if (state == (int)RocketState.FIRING) {
				if (onScored != null) {
					onScored(200);
				}
			} else {
				if (onScored != null) {
					onScored(-50);
				}
			}
			explode();
		}
	}

	bool reachedTarget() {
		float magnitude = (rigidBody.position - targetPosition).magnitude;
		return (magnitude <= triggerDistance);
	}

	void explode() {
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.transform.position = transform.position;
		explosion.tag = tag; // the explosion should act as a proxy for this GameObject
		Destroy(gameObject);
	}

	protected override void onStateChange() {
		switch ((RocketState)state) {
		case RocketState.IDLE:
			break;
		case RocketState.FIRING:
			break;
		}
	}

	void onGameEnded() {
		Destroy(gameObject);
	}

	public void setTarget(Vector3 target) {
		// set up initial target position and angle
		targetPosition = target;
		Vector3 direction = transform.position - target;
		direction.Normalize();
		targetAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90.0f;
		// kick the rocket off the pad
		rigidBody.velocity = new Vector2(0, initialVelocity);
		// switch to FIRING
		setState((int)RocketState.FIRING);
	}

	public void destroy() {
		Destroy(gameObject);
	}

}
