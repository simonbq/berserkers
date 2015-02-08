using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {

	public GameObject explosion;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Player"){
			PickUp();
		}
	}

	public void PickUp () {
		Explode ();
		Destroy (gameObject);
	}

	void Explode() {
		Instantiate (explosion, transform.position, transform.rotation);
	}
}
