using UnityEngine;
using System.Collections;

public class ScaleByCam : MonoBehaviour {
	public Camera cam;
	
	// Update is called once per frame
	void Update () {
		Debug.Log(cam.pixelWidth);
		Vector3 v = Vector3.one;
		//v.x = cam.pixelWidth / 1920.0f;
		v.x = 1/(cam.pixelHeight / 1080.0f);
		transform.localScale = v;
	}
}
