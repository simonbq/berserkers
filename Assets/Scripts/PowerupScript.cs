using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
	public float speedIncrease = 0.05f;
	public GameObject explosion;
    public GameObject spawnPoint;

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("DeadPlayer"), LayerMask.NameToLayer("Ragdoll"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("DeadPlayer"), LayerMask.NameToLayer("DeadPlayer"));
        
	}

	// Update is called once per frame
	void Update () {
        if (!Connections.GetInstance().isConnected)
        {
            Destroy(this.gameObject);
        }
	}

	void Awake () {
        if (Application.loadedLevelName == "Lobby" || !Connections.GetInstance().isConnected)
            Destroy(this.gameObject);
        else
		SoundStore.instance.Play (SoundStore.instance.PowerUpSpawn);
	}

	void OnTriggerEnter(Collider c){
		if(Network.isServer &&
		   c.gameObject.tag == "Player"){
			PlayerController player = c.gameObject.GetComponent<PlayerController>();
            
            player.AddSpeed(speedIncrease);

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

		SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpSound);
		SoundStore.instance.PlayRandom (SoundStore.instance.PowerUpPickUpShout);

		if(Connections.GetInstance().localPlayers.Exists(x => x.id == id))
		{
			ScreenShaker.instance.Shake (1, 0.2f);
		}
	}

    void OnDestroy()
    {
        if (Network.isServer)
        {
            GameController.instance.ReleaseRuneSpawn(spawnPoint);
        }
    }
}
