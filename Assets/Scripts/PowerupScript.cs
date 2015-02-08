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
			PlayerController player = c.gameObject.GetComponent<PlayerController>();
			player.movementSpeed += speedIncrease;
			PickUp (player.playerInfo.id);
		}
	}
	
	void PickUp (int id) {
		networkView.RPC ("Explode", RPCMode.All, id);
		Network.Destroy (gameObject);
	}

	[RPC]
	void Explode(int id) {
		Instantiate (explosion, transform.position, transform.rotation);

		if(Connections.GetInstance().playerId == id)
		{
			SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpSound);
			SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpShout);

			ScreenShaker.instance.Shake (1, 0.2f);
		}
	}
}
