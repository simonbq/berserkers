using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public float lookOffset;
	public float ease;
	public Vector3 cameraOffset;
	public PlayerController player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(player != null)
		{
			Vector3 targetPos = player.transform.position + cameraOffset + player.transform.forward * lookOffset;
			transform.position = Vector3.Lerp (transform.position, targetPos, ease * Time.fixedDeltaTime);
		}
	}
}
