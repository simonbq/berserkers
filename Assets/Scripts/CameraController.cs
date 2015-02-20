using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float lookOffset;
	public float ease;
	public Vector3 cameraOffset;
	public List<PlayerController> toFollow = new List<PlayerController>();
	private bool thirdPerson = false;
	private Quaternion originalRotaion;

	// Use this for initialization
	void Start () {
		originalRotaion = transform.rotation;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Minus)) {
			if (thirdPerson)
				transform.rotation = originalRotaion;
			thirdPerson = !thirdPerson;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!thirdPerson) {
			var follow = toFollow.FindAll (x => x.state != PlayerController.PlayerState.DEAD);

			Vector3 targetPos = Vector3.zero;
            float avgSpeed = 0;
			foreach(PlayerController player in follow)
			{
				targetPos += player.transform.position;
                avgSpeed += player.currentSpeed;
			}
			targetPos /= follow.Count;
            avgSpeed /= follow.Count;

			float dist = 0;
			foreach(PlayerController player in follow)
			{
				dist += Vector3.Distance(player.transform.position, targetPos);
			}

			dist /= follow.Count;
			targetPos += cameraOffset;
			targetPos.y += dist * 0.7f;
			targetPos.z -= dist * 0.66f;

			targetPos = Vector3.Lerp (Vector3.zero, targetPos, lookOffset);

		    if (!toFollow.Exists (x => x.state != PlayerController.PlayerState.DEAD))
		    {
		        targetPos = Vector3.zero + Vector3.forward * cameraOffset.z * 1.5f + Vector3.up * 3f + cameraOffset;
		    }

			transform.position = Vector3.Lerp (transform.position, targetPos, (ease + ease * (avgSpeed / 0.55f)) * Time.fixedDeltaTime);
		}
		else {
			transform.position = toFollow[0].cam.position;
			transform.rotation = toFollow[0].cam.rotation;
		}
	}
}
