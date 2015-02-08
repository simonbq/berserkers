using UnityEngine;
using System.Collections;

public class spotlightScript : MonoBehaviour {
	public AnimationCurve ac;
	public float duration = 5.0f;
	public float length = 20.0f;

	private float elapsed = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float force = 90 + ac.Evaluate (elapsed / duration) * length;
		Vector3 rot = new Vector3(force, 0 ,0);
		transform.localRotation = Quaternion.Euler(rot);

		elapsed += Time.fixedDeltaTime;
		if (elapsed > duration) {
			elapsed = 0.0f;
		}
	}
}
