using UnityEngine;
using System.Collections;

public class ScreenShaker : MonoBehaviour {
	public static ScreenShaker instance { get; private set; }
	//private float intensity;
	//private float duration;
	//private float elapsed = 0.0f;
	private float tick;
	public float constIntensity { get; private set; }
	public AnimationCurve shakeFalloff;
	public bool testMode = false;
	//private bool shaking = false;
	// Use this for initialization
	void Awake () {
		instance = this;
		tick = Time.time;
		constIntensity = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(testMode && tick < Time.time) {
			tick = Time.time + Random.Range(0.1f, 2.0f);
			Shake (Random.Range(0.0f, 0.5f), Random.Range(0.0f, 6.0f));
		}
		transform.position = Random.insideUnitSphere.normalized * constIntensity;
	}

	private IEnumerator ShakeNBake(float intensity, float duration) {
		bool shaking = true;
		float elapsed = 0.0f;
		while(shaking) {
			float force = shakeFalloff.Evaluate(elapsed / duration) * intensity;
			transform.position += Random.insideUnitSphere.normalized * force;
			
			elapsed += Time.fixedDeltaTime;
			if (elapsed > duration) {
				shaking = false;
			}
			yield return null;
		}
	}

	public void Shake(float intensity = 1.0f, float duration = 1.0f) {
		StartCoroutine (ShakeNBake(intensity, duration));

	}

	public void SetConstantShakyness(float intensity) {
		constIntensity = intensity;
	}
}
