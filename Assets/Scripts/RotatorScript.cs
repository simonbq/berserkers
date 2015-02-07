using UnityEngine;
using System.Collections;

public class RotatorScript : MonoBehaviour {
	public float rotationspeed = 0.4f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate (new Vector3 (0, rotationspeed, 0));
	}
}
