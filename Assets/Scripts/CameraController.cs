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
			Vector3 targetPos = player.transform.position + cameraOffset;
			targetPos = Vector3.Lerp (Vector3.zero, targetPos, lookOffset);

            if (player.state == PlayerController.PlayerState.DEAD)
            {
                targetPos = Vector3.zero + Vector3.up * 3f + cameraOffset;
            }

			transform.position = Vector3.Lerp (transform.position, targetPos, ease * Time.fixedDeltaTime);
		}
	}
}
