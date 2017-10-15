using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBehavior : MonoBehaviour {

	protected int state = -1;

	protected void setState(int newState) {
		if (state != newState) {
			state = newState;
			onStateChange();
		}
	}

	protected virtual void onStateChange() {
		Debug.Log("unhandled state change: " + state.ToString());
	}
	
}
