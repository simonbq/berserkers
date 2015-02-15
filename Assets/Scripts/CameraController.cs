using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float lookOffset;
	public float ease;
	public Vector3 cameraOffset;
	public List<PlayerController> toFollow = new List<PlayerController>();
	private bool thirdPerson = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!thirdPerson) {
			Vector3 targetPos = Vector3.zero;
			foreach(PlayerController player in toFollow)
			{
				targetPos += player.transform.position;
			}
			targetPos /= toFollow.Count;

			float dist = 0;
			foreach(PlayerController player in toFollow)
			{
				dist += Vector3.Distance(player.transform.position, targetPos);
			}

			dist /= toFollow.Count;
			targetPos += cameraOffset;
			targetPos.y += dist * 0.5f;
			targetPos.z -= dist * 0.5f;

			targetPos = Vector3.Lerp (Vector3.zero, targetPos, lookOffset);

		    if (!toFollow.Exists (x => x.state != PlayerController.PlayerState.DEAD))
		    {
		        targetPos = Vector3.zero + Vector3.forward * cameraOffset.z * 1.5f + Vector3.up * 3f + cameraOffset;
		    }

			transform.position = Vector3.Lerp (transform.position, targetPos, ease * Time.fixedDeltaTime);
		}
		else {
			transform.position = toFollow[0].cam.position;
			transform.rotation = toFollow[0].cam.rotation;
		}
	}
}
