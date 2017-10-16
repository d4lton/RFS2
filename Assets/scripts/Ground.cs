using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	void Start() {
	}
	
	void Update() {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		Vector3 impactPosition = collider.gameObject.GetComponent<Rigidbody2D>().transform.position;
		//Debug.Log("collision" + impactPosition.ToString());
	}

}
