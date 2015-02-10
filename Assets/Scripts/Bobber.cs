﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bobber : MonoBehaviour {
	public static Bobber instance { get; private set; }

	private List<Transform> crowds = new List<Transform>();
	private bool[] occupied;
	public float bobTimer = 0.1f;
	private float lastBob = 0.0f;
	public int bobsPerTick = 5;

	public int cheerFactor { get; set; }

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
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > lastBob + bobTimer) {
			lastBob = Time.time + bobTimer;
			for(int i = 0; i < bobsPerTick * cheerFactor; i++) {
				int r = Random.Range(0, crowds.Count);
				if(!occupied[r]) {
					StartCoroutine(bob (crowds[r], r));
					occupied[r] = true;
				}
			}
		}
	}


	IEnumerator bob(Transform t, int i) {
		float startTime = Time.time;
		Vector3 start = t.position;
		while(Time.time - startTime < Mathf.PI) {
			t.position = start + Vector3.up * Mathf.Abs(Mathf.Sin((Time.time-startTime) * 2)) / 4;
			yield return null;
		}
		t.position = start;
		occupied [i] = false;
		yield return null;
	}
}
