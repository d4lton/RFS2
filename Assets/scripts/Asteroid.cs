using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour {

	Rigidbody2D rigidBody;

	public GameObject explosionPrefab;
	public float yStart = 7.0f;
	public float xStartOffset = 3.0f;
	public float xForce = 80.0f;
	public float yParticleShutoff = 1.0f;
	public float yParticleDecayRate = 0.01f;
	public float difficultyDownForce = 30.0f;
	public float sideForceRate = 1.5f;

	ParticleSystem particles;
	bool particlesEnabled = false;
	float sideForce;

	void Start() {
		rigidBody = GetComponent<Rigidbody2D>();

		// position the asteroid at its starting x position
		float xStart = Random.Range(-xStartOffset, xStartOffset);
		rigidBody.transform.position = new Vector3((float)xStart, yStart, 0f);

		// ===========================
		// TODO: screw all this noise, just pick a target on the ground and Lerp towards it, something like how we Lerp towards the mouse for the Rocket
		// ===========================

		// try really hard to keep the asteroid in the playing field by adjusting its side force depending on how far to the left or right it starts
		float percentageOffCenter = -(xStart / xStartOffset);
		float adjustment = percentageOffCenter * xForce;
		sideForce = Random.Range((-xForce) + adjustment, xForce + adjustment);

		rigidBody.AddForce(Vector2.right * sideForce);

		// depending on the difficulty level, add (or increase) downward initial force to make the asteroid zip into the field at higher levels
		int difficulty = GameManager.instance.getDifficultyLevel();
		float downForce = difficultyDownForce * difficulty;
		rigidBody.AddForce(Vector2.down * downForce);

		// hold onto some particle-related info, we'll be using it in Update to turn off the fireball effect once the asteroids are in the lower atmosphere
		particles = GetComponentInChildren<ParticleSystem>();
		particlesEnabled = true;
	}
	
	void Update() {

		// rotate the asteroid towards its direction of movement, mainly to make the particle effects look right
		if (rigidBody.velocity != Vector2.zero) {
			float angle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
		}

		// apply the side force
		//rigidBody.AddForce(Vector2.right * (sideForce * Time.deltaTime * sideForceRate));

		// turn off the fireball effect if we're below a certain altitude
		if (particlesEnabled) {
			if (rigidBody.position.y < yParticleShutoff) {
				particlesEnabled = false;
				ParticleSystem.EmissionModule emission = particles.emission;
				emission.rateOverTime = Time.deltaTime * yParticleDecayRate;
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

	private void explode() {
		GameObject explosion = Instantiate(explosionPrefab) as GameObject;
		explosion.GetComponent<Explosion>().disableColliders();
		explosion.transform.position = transform.position;
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		explode();
	}

}
