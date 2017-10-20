using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour {

	Rigidbody2D rigidBody;

	public GameObject explosionPrefab;
	public float yStart = 7.0f;
	public float yEnd = -5.0f;
	public float xStartOffset = 3.0f;
	public float yParticleShutoff = 3.0f;
	public float yParticleDecayRate = 0.01f;
	public float difficultyDownForce = 20.0f;
	public float downForce = 60.0f;

	ParticleSystem particles;
	bool particlesEnabled = false;
	float sideForce;
	int difficultyLevel;

	float targetAngle;
	Vector3 targetPosition;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();

		// hold onto some particle-related info, we'll be using it in Update to turn off the fireball effect once the asteroids are in the lower atmosphere
		particles = GetComponentInChildren<ParticleSystem>();
		particlesEnabled = true;

		difficultyLevel = GameManager.instance.getDifficultyLevel();

		setupInitialPosition();
	}
	
	void Update() {

		// accelerate towards the target (taking into account the difficulty level, to speed up the asteroid)
		float force = downForce + (difficultyLevel * difficultyDownForce);
		rigidBody.AddForce(rigidBody.transform.up * Time.deltaTime * -force);

		// turn off the fireball effect if we're below a certain altitude
		if (particlesEnabled) {
			if (rigidBody.position.y < yParticleShutoff) {
				particlesEnabled = false;
				ParticleSystem.EmissionModule emission = particles.emission;
				emission.rateOverTime = 0f;
			}
		}

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
	void setupInitialPosition() {

		// position the asteroid at its starting x position
		float xStart = Random.Range(-xStartOffset, xStartOffset);
		rigidBody.transform.position = new Vector3((float)xStart, yStart, 0f);

		// find a target on the ground
		if (Random.Range(0, difficultyLevel) == 0) {
			Debug.Log("TARGETING RANDOMLY");
			targetRandomly();
		} else {
			Debug.Log("TARGETING SPECIFICALLY");
			if (!targetSpecifically()) {
				Debug.Log("TARGETING RANDOMLY BECAUSE SPECIFIC TARGETING FAILED");
				targetRandomly();
			}
		}

		// rotate the asteroid towards the target
		Vector3 direction = transform.position - targetPosition;
		direction.Normalize();
		targetAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
		transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

	}

	void targetRandomly() {
		targetPosition = new Vector3(Random.Range(-xStartOffset, xStartOffset), yEnd, 0f);
	}

	bool targetSpecifically() {
		List<GameObject> pads = new List<GameObject>();
		pads.AddRange(GameObject.FindGameObjectsWithTag("Pad"));
		pads.AddRange(GameObject.FindGameObjectsWithTag("BrokenPad"));
		if (pads.Count > 0) {
			int padIndex = Random.Range(0, pads.Count);
			targetPosition = pads[padIndex].transform.position;
			return true;
		} else {
			return false;
		}
	}

	void explode() {
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.GetComponent<Explosion>().disableColliders();
		explosion.transform.position = transform.position;
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		explode();
	}

}
