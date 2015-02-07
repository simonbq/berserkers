using UnityEngine;
using System.Collections;

public class SpawnPointController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawCube(transform.position, new Vector3(1,0.2f,1));
		Gizmos.DrawRay(transform.position, transform.forward * 3);
	}
}
