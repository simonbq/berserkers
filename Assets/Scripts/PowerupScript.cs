using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
	public float speedIncrease = 0.05f;
	public GameObject explosion;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision collision){
		if(Network.isServer &&
		   collision.gameObject.tag == "Player"){
			collision.gameObject.GetComponent<PlayerController>().movementSpeed++;
			PickUp ();
		}
	}
	
	void PickUp () {
		Explode ();
		Network.Destroy (gameObject);
	}

	void Explode() {
		Instantiate (explosion, transform.position, transform.rotation);
	}
}
