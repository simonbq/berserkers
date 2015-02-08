using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public enum PlayerState { ALIVE = 0, DEAD, STUNNED };
	public PlayerState state;
	private PlayerInfo _playerInfo;
	public PlayerInfo playerInfo
	{
		get
		{
			return _playerInfo;
		}

		set
		{
			_playerInfo = value;
			if(_playerInfo.id == Connections.GetInstance().playerId)
			{
				CameraController cc = Camera.main.GetComponent<CameraController>();
				
				if(cc != null)
				{
					cc.player = this;
				}
			}
		}
	}

	//Player variables
    public float movementSpeed;
	public float turnSpeed;

    public Material[] materials;
    public enum PlayerColor { BLACK, BLUE, BROWN, GREEN, ORANGE, PINK, PURPLE, RED };

    public float stunDuration;

    public Animator animator;
    public Material playerMaterial;

	private float _input = 0;
	private float startSpeed;
	private float currentSpeed = 0;

	private bool firstblood = false;
	private Vector3 netPosition = Vector3.zero;

	private float input
	{
		set
		{
			if(_input != value)
			{
				networkView.RPC ("UpdateInput", RPCMode.All, Network.player, value);
			}
		}
	}

	// Use this for initialization
	void Start()
	{
		startSpeed = movementSpeed;

		Reset ();

        //GameObject ninja = GameObject.Find(transform.name + "/ninja");


        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.transform.name != "Blood Particle System")
                r.material = materials[playerInfo.id];
        }
        movementSpeed = startSpeed;
        currentSpeed = 0;
	}

	public void Reset () {
		state = PlayerState.STUNNED;
        animator.SetBool("idle", true);
        Invoke("MakeAlive", 2.0f);

		Invoke ("AnnouncerStart", 2.0f);
		Debug.Log ("Should play sound for round start soon");

		//renderer.material.color = playerColor;
		movementSpeed = startSpeed;
		currentSpeed = 0;

		networkView.RPC ("ForcePosition", RPCMode.All, transform.position);

	}

    void Update()
    {
        if (!playerInfo.connected)
        {
            Destroy(this.gameObject);
        }
        
        if (CheckNearbyPlayers(4.0f))
        {
            Debug.Log(gameObject.name + "Found nearby player, setting enemyclose true");
            animator.SetBool("enemyclose", true);
        }
        else
        {
            Debug.Log(gameObject.name + "Did not find nearby player, setting enemyclose false");
            animator.SetBool("enemyclose", false);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		Debug.Log ("ID: " + playerInfo.id + " State: " + state);
		if(playerInfo.id == Connections.GetInstance().playerId)
		{
			input = Input.GetAxis("Horizontal");

            //check for inputdelay
            if (Input.GetAxis("Horizontal") == 0 &&
                Input.GetAxis("Horizontal") != _input)
            {
                Debug.Log("Actual: " + Input.GetAxis("Horizontal") + " Server: " + _input);
            }
		}

        if (Network.isServer)
        {
            if (state == PlayerState.ALIVE)
            {
                rigidbody.MovePosition(transform.position + transform.forward * currentSpeed);
				currentSpeed = Mathf.Lerp (currentSpeed, movementSpeed, Time.fixedDeltaTime);

                transform.Rotate(Vector3.up, _input * turnSpeed);
            }

			else
			{
				currentSpeed = 0;
			}
        }

        else if(state == PlayerState.ALIVE)
        {
			Vector3 targetPos = Vector3.Lerp (transform.position, netPosition, Time.fixedDeltaTime);

            transform.position = Vector3.MoveTowards(targetPos, targetPos + transform.forward * currentSpeed, currentSpeed);
			currentSpeed = Mathf.Lerp (currentSpeed, movementSpeed, Time.fixedDeltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.eulerAngles + Vector3.up * turnSpeed * _input), turnSpeed);
        }

		if(state == PlayerState.DEAD)
		{
			renderer.material.color = Color.black;
			//die
		}
	}


    void OnCollisionEnter(Collision collision)
    {
        if (Network.isServer &&
            state != PlayerState.DEAD)
        {
            if (collision.gameObject.tag == "Player")
            {
                animator.SetTrigger("attack");
                Debug.Log("Set trigger attack");

                PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
				Debug.Log ("Hit player " + hitPlayer.name);

                if (hitPlayer.currentSpeed > this.movementSpeed)
                {
                    Debug.Log("Kill player");
                    state = PlayerState.DEAD;
					networkView.RPC ("PlayDeathShout", RPCMode.All);


                    rigidbody.AddExplosionForce(2000, transform.position + transform.forward * 2, 0, 0);

                    networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id);
					Debug.Log ("Killed by " + hitPlayer.playerInfo.id);
                   
                    this.playerInfo.killstreaks.Died();

                    int playersAlive = 0;
                    foreach (GameObject player in GameController.instance.players)
                    {
                        if (player.GetComponent<PlayerController>().state == PlayerState.ALIVE)
                            playersAlive++;
                    }
                    if (playersAlive < 2)
					{
						firstblood = false;
						networkView.RPC ("PlayWinSound", RPCMode.All);
						GameController.instance.Invoke("SpawnPlayers", 3);
                    }
                    if (playersAlive == Connections.GetInstance().players.Count - 1 && !firstblood)
                    {
                        Firstblood();
                    }
                }
                else if (hitPlayer.movementSpeed == this.movementSpeed &&
                         state != PlayerState.STUNNED)
                {
                    Stunned(stunDuration, false, collision.contacts[0].normal);
                }
            }

			if (collision.gameObject.tag == "Wall" &&
			    state != PlayerState.STUNNED)
			{
				Stunned(stunDuration, true, collision.contacts[0].normal);
			}
		}
	}
	
	public void SetPlayer(int id)
	{
		networkView.RPC ("Spawned", RPCMode.All, id);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int playerState = 0;
		float mSpeed = 0;
		float cSpeed = 0;
		Vector3 position = new Vector3();
		Vector3 velocity = new Vector3 ();
		Quaternion rotation = Quaternion.identity; 

		if(stream.isWriting)
		{
			playerState = (int)state;
			mSpeed = movementSpeed;
			cSpeed = currentSpeed;
			position = transform.position;
			velocity = rigidbody.velocity;
			rotation = transform.rotation;

			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref cSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref rotation);
		}

		else
		{
			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref cSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref rotation);

			state = (PlayerState)playerState;
			if(state != PlayerState.ALIVE)
			{
				rigidbody.velocity = velocity;
			}

			movementSpeed = mSpeed;
			currentSpeed = cSpeed;
			netPosition = position;
			transform.rotation = rotation;
		}
	}
	

	void Stunned(float duration, bool wall, Vector3 normal)
	{
		//stun stuff here

        animator.SetBool("idle", true);
		state = PlayerState.STUNNED;
        Invoke("MakeAlive", duration);
		transform.forward = Vector3.Reflect (transform.forward, normal);


		rigidbody.AddExplosionForce(750, transform.position - transform.forward * 2, 0, 0);
		networkView.RPC ("PlayStunnedFX", RPCMode.All, wall);
		movementSpeed = startSpeed;
		currentSpeed = 0;

	}

    /* Check for players within a radius */
    public bool CheckNearbyPlayers(float mRadius)
    {
        bool returnValue = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, mRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.tag == "Player" && c.gameObject != gameObject)
            {
                returnValue = true;
            }

        }


        return returnValue;
    }

    void MakeAlive()
    {
        state = PlayerState.ALIVE;
        animator.SetBool("idle", false);
    }

	void AnnouncerStart()
	{
		SoundStore.instance.Play (SoundStore.instance.AnnouncerStart);
		Debug.Log ("Play round start sound now");
	}

	void Firstblood()
	{
		firstblood = true;
		networkView.RPC ("PlayFirstBlood", RPCMode.All);
	}

	[RPC]
	void ForcePosition(Vector3 pos)
	{
		transform.position = pos;
	}

	[RPC]
	void PlayStunnedFX(bool wall)
	{
		if (Connections.GetInstance ().playerId == playerInfo.id) 
		{
			//ScreenShaker.instance.Shake (1, 1);
		}
		if (Connections.GetInstance ().playerId == playerInfo.id)
		{
			if (wall)
				SoundStore.instance.PlayRandom (SoundStore.instance.StunSoundWall);
			else 
			{
				SoundStore.instance.PlayRandom (SoundStore.instance.StunSoundPlayer);
				SoundStore.instance.PlayRandom (SoundStore.instance.StunShout);
			}
		}
	}

	[RPC]
	void PlayFirstBlood()
	{
		if (Connections.GetInstance ().playerId == playerInfo.id)
			SoundStore.instance.Play (SoundStore.instance.AnnouncerFirstBlood);
	}

	[RPC]
	void PlayDeathShout()
	{
		if (Connections.GetInstance ().playerId == playerInfo.id)
			SoundStore.instance.PlayRandom (SoundStore.instance.DeathShout);
	}

	[RPC]
	void PlayWinSound()
	{
		if (Connections.GetInstance ().playerId == playerInfo.id)
			SoundStore.instance.Play (SoundStore.instance.WinSound);
	}

	[RPC]
	void Kill(int killerId)
	{
		Debug.Log (playerInfo.id + " got killed by " + killerId);

		//You can get killerId by killerGameObject.GetComponent<PlayerController>().playerInfo.id
		Connections.GetInstance ().players [killerId].kills++;
		playerInfo.deaths++;

		Killstreaks killerKillstreak = Connections.GetInstance ().players [killerId].killstreaks;

		if (Connections.GetInstance ().playerId == playerInfo.id)
		{
			killerKillstreak.AddKill ();
			SoundStore.instance.PlayRandom (SoundStore.instance.KillSound);
			SoundStore.instance.PlayRandom (SoundStore.instance.KillShout);
			if (killerKillstreak.GetFastKills() == 2) {
				SoundStore.instance.Play(SoundStore.instance.AnnouncerDoubleKill);
			}
			else if (killerKillstreak.GetFastKills() == 3) {
				SoundStore.instance.Play(SoundStore.instance.AnnouncerMultiKill);
			}
			else if (killerKillstreak.GetKills() == 3) {
				SoundStore.instance.Play(SoundStore.instance.AnnouncerThreeKills);
			}
			else if (killerKillstreak.GetKills() == 5) {
				SoundStore.instance.Play(SoundStore.instance.AnnouncerFiveKills);
			}
			else if (killerKillstreak.GetKills() == 7) {
				SoundStore.instance.Play(SoundStore.instance.AnnouncerSevenKills);
			}
		}

		if (Connections.GetInstance ().playerId == playerInfo.id) 
		{
			//ScreenShaker.instance.Shake (1, 1);
		}
	}

	[RPC]
	void UpdateInput(NetworkPlayer sender, float val)
	{
		_input = val;
	}

	[RPC]
	void Spawned(int id)
	{
		playerInfo = Connections.GetInstance ().players [id];
	}
}
