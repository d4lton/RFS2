using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPad : MonoBehaviour {

	void Start() {
	}
	
	void Update() {
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

}
