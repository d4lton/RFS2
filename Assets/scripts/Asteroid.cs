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

	ParticleSystem particles;
	bool particlesEnabled = false;

	void Start() {
		Debug.Log("DIFFICULTY: " + GameManager.instance.getDifficultyLevel());
		rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.transform.position = new Vector3((float)Random.Range(-xStartOffset, xStartOffset), yStart, 0f);
		rigidBody.AddForce(Vector2.right * (float)Random.Range(-xForce, xForce), ForceMode2D.Force);

		int difficulty = GameManager.instance.getDifficultyLevel();
		if (difficulty > 10) {
			difficulty = 10;
		}
		float downForce = difficultyDownForce * difficulty;
		rigidBody.AddForce(Vector2.down * downForce);

		particles = GetComponentInChildren<ParticleSystem>();
		particlesEnabled = true;
	}
	
	void Update() {
		if (rigidBody.velocity != Vector2.zero) {
			float angle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
		}
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
		if (collider.gameObject.tag == "Ground") {
			explode();
			// TODO: hitting the ground should incur a penalty, like burning for awhile and preventing rocket reload?
		}
		if (collider.gameObject.tag == "Rocket") {
			explode();
		}
	}

}
