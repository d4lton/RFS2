using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {

	public Text scoreText;

	public static Level instance;

	void Awake() {
		instance = this;
	}

	void Start() {
		setLevel(0);
	}

	public void setLevel(int level) {
		int displayLevel = level + 1;
		scoreText.text = "LEVEL " + displayLevel.ToString();
	}

}
