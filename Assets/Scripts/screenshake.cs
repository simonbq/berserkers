using UnityEngine;
using System.Collections;

public class screenshake : MonoBehaviour {
	public float intensity = 1.0f;
	public float duration = 1.0f;
	private float elapsed = 0.0f;
	public AnimationCurve ac;
	bool shaking = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (shaking) {
			float force = ac.Evaluate(elapsed / duration) * intensity;
			transform.position = Random.insideUnitSphere.normalized * force;

			elapsed += Time.fixedDeltaTime;
			if (elapsed > duration) {
				transform.position = Vector3.zero;
				shaking = false;
			}
		}
	}

	public void Shake() {
		elapsed = 0.0f;
		shaking = true;
	}
}