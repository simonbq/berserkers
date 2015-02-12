using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bobber : MonoBehaviour {
	public static Bobber instance { get; private set; }

	private List<Transform> crowds = new List<Transform>();
	private bool[] occupied;
	public float bobTimer = 0.1f;
	private float lastBob = 0.0f;
	public float bobsPerTick = 5;
	public bool crowdAudioEnabled = true;

	private AudioSource[] mood;
	public float cheerFactor { get; set; }

	// Use this for initialization
	void Awake () {
		instance = this;
		cheerFactor = 1;
		for(int i = 0; i < transform.childCount; i++) {
			Transform tI = transform.GetChild(i);
			for(int j = 0; j < tI.childCount; j++) {
				Transform tJ = tI.GetChild(j);
				for(int n = 0; n < tJ.childCount; n++)  {
					Transform tN = tJ.GetChild(n);
					for(int k = 0; k < tN.childCount; k++) {
						Transform tK = tN.GetChild(k);
						crowds.Add(tK);
					}
				}
			}
		}
		occupied = new bool[crowds.Count];
		mood = GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > lastBob + bobTimer) {
			lastBob = Time.time + bobTimer;
			for(int i = 0; i < bobsPerTick * cheerFactor; i++) {
				int r = Random.Range(0, crowds.Count);
				if(!occupied[r]) {
					StartCoroutine(bob (crowds[r], r, Random.Range(1, 3)));
					occupied[r] = true;
				}
			}
		}
		if(crowdAudioEnabled) {
			mood [0].volume = Mathf.InverseLerp (0, 10, bobsPerTick * cheerFactor);
			mood [1].volume = Mathf.InverseLerp (20, 50, bobsPerTick * cheerFactor);
		}
	}


	IEnumerator bob(Transform t, int i, int jumps = 2, float height = 1.0f) {
		float startTime = Time.time;
		Vector3 start = t.position;
		while(Time.time - startTime < Mathf.PI / 2 * jumps) {
			t.position = start + (Vector3.up * Mathf.Abs(Mathf.Sin((Time.time-startTime) * 2)) / 4) * height;
			yield return null;
		}
		t.position = start;
		occupied [i] = false;
		yield return null;
	}

	public void startClimax(float startIntensity, float duration) {
		StartCoroutine (climax (startIntensity, duration));
	}

	private IEnumerator climax(float startIntensity, float duration) {
		float intensity = startIntensity;
		float start = Time.time;
		while(Time.time < start + duration) {
			intensity = Mathf.Lerp(startIntensity, 1.0f, (Time.time - start) / duration);
			for(int i = 0; i < bobsPerTick * intensity; i++) {
				int r = Random.Range(0, crowds.Count);
				if(!occupied[r]) {
					StartCoroutine(bob (crowds[r], r, Random.Range(1, 5), intensity));
					occupied[r] = true;
				}
			}
			yield return new WaitForSeconds(bobTimer);
		}
	}
}
