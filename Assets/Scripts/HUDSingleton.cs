using UnityEngine;
using System.Collections;

public class HUDSingleton : MonoBehaviour {

	public static HUDSingleton instance { get; private set; }

	public float speed { get; set; }


	void Awake () {
		instance = this;
	}
}
