using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	public delegate void ScoreDelegate(int value);
	public static event ScoreDelegate onScored;

	void OnTriggerEnter2D(Collider2D collider) {
		//Vector3 impactPosition = collider.gameObject.GetComponent<Rigidbody2D>().transform.position;
		if (collider.tag == "Asteroid") {
			Debug.Log("ASTEROID HIT GROUND");
			if (onScored != null) {
				onScored(-20);
			}
		}
	}

}
