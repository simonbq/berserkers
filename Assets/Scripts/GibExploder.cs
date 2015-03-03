using UnityEngine;
using System.Collections;

public class GibExploder : MonoBehaviour {
	public float force = 1000f;
	
	public void Explode(Material mat) {
		foreach(Transform t in transform)
		{
			foreach(Transform g in t)
			{
				if(g.renderer != null &&
				   g.particleSystem	== null)
				{
					g.renderer.material = mat;
				}
			}
			t.rigidbody.AddExplosionForce(force, transform.position - transform.up * 2 - new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)), 30);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
