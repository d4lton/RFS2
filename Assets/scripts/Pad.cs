using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {

	public delegate void PadDelegate();
	public event PadDelegate onPadDestroyed;

	public GameObject rocketPrefab;

	void Start() {
	}

	void Update() {
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Asteroid") {
			if (onPadDestroyed != null) {
				onPadDestroyed();
			}
			Destroy(gameObject);
		}
	}

	public bool hasRocket() {
		// TODO: yeah.
		return true;
	}

	public void fire(Vector3 target) { // target is Input.mousePosition

		GameObject rocket = Instantiate(rocketPrefab) as GameObject;
		rocket.transform.position = transform.position;
		rocket.GetComponent<Rocket>().setTarget(target);

		// TODO: these should be done when rocket is first added to the pad (somewhere else)
		// TODO: we'll also need to store the rocket object
		// TODO: another thing might be to move this "fire" code into Rocket itself
		//GameObject rocket = Instantiate(rocketPrefab);
		//rocket.transform.position = transform.position;

		//Vector3 screenPoint = Camera.main.WorldToScreenPoint(rocket.transform.localPosition);
		//Vector2 offset = new Vector2(target.x - screenPoint.x, target.y - screenPoint.y);
		//float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
		//Debug.Log(angle);

		//rocket.GetComponent<Rocket>().setTargetAngle(angle);

		// TODO: this rocket should already have been position, so it just needs to be targeted
	
	}

}
