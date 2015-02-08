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

	void OnTriggerEnter(Collider c){
		if(Network.isServer &&
		   c.gameObject.tag == "Player"){
			c.gameObject.GetComponent<PlayerController>().movementSpeed += speedIncrease;
			PickUp ();
			SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpSound);
			SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpShout);

		}
	}
	
	void PickUp () {
		networkView.RPC ("Explode", RPCMode.All);
		Network.Destroy (gameObject);
	}

	[RPC]
	void Explode() {
		Instantiate (explosion, transform.position, transform.rotation);
	}
}
