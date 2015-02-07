using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float movementSpeed;
	public float turnSpeed;
	public Color playerColor;
	public AudioClip[] audioClips;

	// Use this for initialization
	void Start () {
		renderer.material.color = playerColor;
	}
	
	// Update is called once per frame
	void Update () {

		rigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Input.GetAxis("Horizontal") * turnSpeed, transform.eulerAngles.z);
		/*
		if(Input.GetKey (KeyCode.A)){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 1 * turnSpeed, transform.eulerAngles.z);
		}
		if(Input.GetKey (KeyCode.D)){
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 1 * turnSpeed, transform.eulerAngles.z);
		}*/
	}
}
