using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	ParticleSystem p;
	// Use this for initialization
	void Awake () {
		p = GetComponentInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!p.isPlaying) {
			Destroy(gameObject);
		}
	}
}
