using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public float lookOffset;
	public float ease;
	public Vector3 cameraOffset;
	public List<PlayerController> toFollow = new List<PlayerController>();
	private bool thP = false;
	private Quaternion originalRotaion;
	private int ts = 0;

	// Use this for initialization
	void Start () {
		originalRotaion = transform.rotation;
	}

	void Update() {
		if (!thP) {
			if (ts == 0 && Input.GetKeyDown(KeyCode.S)) ts++;
			else if (ts == 1 && Input.GetKeyDown(KeyCode.H)) ts++;
			else if (ts == 2 && Input.GetKeyDown(KeyCode.U)) ts++;
			else if (ts == 3 && Input.GetKeyDown(KeyCode.T)) ts++;
			else if (ts == 4 && Input.GetKeyDown(KeyCode.U)) ts++;
			else if (ts == 5 && Input.GetKeyDown(KeyCode.P)) ts++;
			else if (ts == 6 && Input.GetKeyDown(KeyCode.U)) ts++;
			else if (ts == 7 && Input.GetKeyDown(KeyCode.K)) ts++;
			else if (ts == 8 && Input.GetKeyDown(KeyCode.N)) ts++;
			else if (ts == 9 && Input.GetKeyDown(KeyCode.O)) ts++;
			else if (ts == 10 && Input.GetKeyDown(KeyCode.W)) {
				ts = 0;
				thP = true;
			}
		}
		else {
			if (ts == 0 && Input.GetKeyDown(KeyCode.F)) ts++;
			else if (ts == 1 && Input.GetKeyDown(KeyCode.U)) {
				ts = 0;
				thP = false;
				transform.rotation = originalRotaion;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!thP) {
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
			targetPos.y += dist * 0.33f;
			targetPos.z -= dist * 0.5f;

			targetPos = Vector3.Lerp (Vector3.zero, targetPos, lookOffset);

		    if (follow.Count == 0)
		    {
		        targetPos = Vector3.zero + Vector3.forward * cameraOffset.z + Vector3.up * 3f + cameraOffset;
		    }

			float extra = ease * (avgSpeed / 0.55f);
            extra = Mathf.Max(0, ease);
            transform.position = Vector3.Lerp (transform.position, targetPos, (ease + extra) * Time.fixedDeltaTime);
		}
		else {
			transform.position = toFollow[0].cam.position;
			transform.rotation = toFollow[0].cam.rotation;
		}
	}
}
