using UnityEngine;
using System.Collections;

public class TimedLife : MonoBehaviour {

	public float lifeTime;
	private float startTime;
	// Use this for initialization
	void Awake () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime > lifeTime) {
			GameObject.Destroy(gameObject);
		}
	}
}
