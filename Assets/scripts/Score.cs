using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	public Text scoreText;

	public static Score instance;

	void Awake() {
		instance = this;
	}

	void Start() {
		setScore(0);
	}
	
	void Update() {
	}

	public void setScore(int score) {
		scoreText.text = "$" + score.ToString();
	}

}
