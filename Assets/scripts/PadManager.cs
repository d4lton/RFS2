using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadManager : MonoBehaviour {

	public delegate void PadManagerDelegate();
	public static event PadManagerDelegate onPadDestroyed;

	void Start() {
	}
	
	void Update() {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		onPadDestroyed();
		Destroy(gameObject);
	}

}
