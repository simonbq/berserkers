using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public enum PlayerState { ALIVE = 0, DEAD, STUNNED, IDLE };
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
			if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
			{
				CameraController cc = Camera.main.GetComponent<CameraController>();
				
				if(cc != null)
				{
					cc.toFollow.Add (this);
				}
			}
		}
	}

	//Player variables
    public float movementSpeed;
	public float turnSpeed;

    private AudioSource mAudioSource;

    public float OVERKILLSPEED = 0.35f;

    public Material[] materials;
    public enum PlayerColor { BLACK, BLUE, BROWN, GREEN, ORANGE, PINK, PURPLE, RED };

    public float stunDuration;

    public Animator animator;
    public Material playerMaterial;
	public GameObject model;
	public GameObject ragdoll;
	public GameObject gibs;
	public GameObject splat;
    public GameObject splatDecal;
    public GameObject nameText;
    public GameObject confetti;

	public Transform cam;

	private float _input = 0;
	private float startSpeed;
	public float currentSpeed = 0;

	private bool firstblood = false;
	private Vector3 netPosition = Vector3.zero;
	private GameObject currentRagdoll;

    private bool inOverKill = false;

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

	void HideModel(bool hidden)
	{
		if(model != null)
		{
			foreach (Transform t in model.transform)
			{
				if(t.renderer != null)
				{
					t.renderer.enabled = !hidden;
				}
			}
		}
	}

	void ShowRagdoll(bool show, bool showGibs)
	{
		if(show && currentRagdoll == null)
		{
			if(showGibs)
			{
				currentRagdoll = Instantiate(gibs, transform.position + transform.up, transform.rotation) as GameObject;
				currentRagdoll.GetComponent<GibExploder>().Explode(materials[playerInfo.id]);
			}

			else
			{
				currentRagdoll = Instantiate (ragdoll, transform.position + transform.up, transform.rotation) as GameObject;
				currentRagdoll.GetComponent<RagdollMaterial> ().SetMaterial (materials [playerInfo.id]);
				currentRagdoll.GetComponent<RagdollMaterial> ().pelvis.rigidbody.AddExplosionForce (25000, transform.position - transform.forward * 2 - transform.up, 30);
			}
		}

		else if(!show)
		{
			Destroy(currentRagdoll);
		}
	}

	// Use this for initialization
	void Start()
	{
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ragdoll"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DeadPlayer"));
        mAudioSource = GetComponent<AudioSource>();
        startSpeed = movementSpeed;
		state = PlayerState.IDLE;
		Reset ();

        SetSpeed(startSpeed);
        currentSpeed = 0;
	}

	public void Reset () {
        CancelInvoke();
		state = PlayerState.IDLE;
        Invoke("MakeAlive", 2.0f);

		Debug.Log ("Should play sound for round start soon");

        rigidbody.velocity = Vector3.zero;
        SetSpeed(startSpeed);
		currentSpeed = 0;

		networkView.RPC ("ForcePosition", RPCMode.All, transform.position);
        gameObject.layer = LayerMask.NameToLayer("Player");
	}

    void Update()
    {
        if (!Connections.GetInstance().isConnected)
        {
            Destroy(this.gameObject);
        }

        if (CheckNearbyPlayers(4.0f))
        {
            animator.SetBool("enemyclose", true);
        }
        else
        {
            animator.SetBool("enemyclose", false);
        }
        CheckOverkill();

    }

	// Update is called once per frame
	void FixedUpdate () {
		Debug.Log (firstblood);
		//Debug.Log ("ID: " + playerInfo.id + " State: " + state);
		if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
		{
			input = Input.GetAxis(playerInfo.input);

            //check for inputdelay
            if (Input.GetAxis("Horizontal") == 0 &&
                Input.GetAxis("Horizontal") != _input)
            {
                Debug.Log("Actual: " + Input.GetAxis("Horizontal") + " Server: " + _input);
            }

			HUDSingleton.instance.speed = currentSpeed;
			//HUDSingleton.instance.onFire = true;
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
			//renderer.material.color = Color.black;
			//die
		}

        switch (state)
        {
            case PlayerState.ALIVE:
                animator.SetBool("idle", false);
                animator.SetBool("stunned", false);
                break;
            case PlayerState.STUNNED:
                animator.SetBool("stunned", true);
                break;
            case PlayerState.IDLE:
                animator.SetBool("stunned", false);
                animator.SetBool("idle", true);
                break;
        }
        animator.SetFloat("speed", movementSpeed);
	}

    //ENTER SUPERSONIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIC
    void CheckOverkill()
    {
        if (movementSpeed >= OVERKILLSPEED && !inOverKill)
        {
            mAudioSource.Play();
            ActivateEffects(true);
            inOverKill = true;

			if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
            {
                HUDSingleton.instance.onFire = true;
                Debug.Log("on fire");
            }
        }

        if (movementSpeed < OVERKILLSPEED && inOverKill)
        {
            mAudioSource.Stop();
            ActivateEffects(false);
            inOverKill = false;

			if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
            {
                HUDSingleton.instance.onFire = false;
            }
        }
    }       

    public void AddSpeed(float mSpeed)
    {
        movementSpeed += mSpeed;
        CheckOverkill();
        if (movementSpeed >= OVERKILLSPEED)
        {
            SoundStore.instance.Play(SoundStore.instance.SonicBoom);
        }
    }
    public void SetSpeed(float mSpeed)
    {
        movementSpeed = mSpeed;
        
        CheckOverkill();
        if (movementSpeed >= OVERKILLSPEED)
        {
            SoundStore.instance.Play(SoundStore.instance.SonicBoom);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Network.isServer &&
            state != PlayerState.DEAD)
        {
            if (collision.gameObject.tag == "Player")
            {
                PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
                if (hitPlayer.state != PlayerState.DEAD)
                {
                    animator.SetTrigger("attack");
                    Debug.Log("Set trigger attack");


                    Debug.Log("Hit player " + hitPlayer.name);

                    if (hitPlayer.currentSpeed > this.movementSpeed)
                    {
                        Debug.Log("Kill player");
                        networkView.RPC("PlayDeathShout", RPCMode.All);

						networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id, Vector3.up, hitPlayer.inOverKill, false);

                    }
                    else if (hitPlayer.movementSpeed == this.movementSpeed &&
                             state != PlayerState.STUNNED)
                    {
                        if (hitPlayer.inOverKill && this.inOverKill)
                        {
                            networkView.RPC("PlayDeathShout", RPCMode.All);

							networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id, Vector3.up, true, false);
							hitPlayer.networkView.RPC("Kill", RPCMode.All, playerInfo.id, Vector3.up, true, false);
                        }
                        else
                        {
                            Stunned(stunDuration, false, collision.contacts[0].normal);
                            hitPlayer.Stunned(stunDuration, false, collision.contacts[0].normal);
                        }
                    }
                }
            }

			if (collision.gameObject.tag == "Wall" &&
			    state != PlayerState.STUNNED)
			{
                if (inOverKill)
                {
					networkView.RPC("Kill", RPCMode.All, playerInfo.id, collision.contacts[0].normal, true, true);
                }
                else
                {
                    Stunned(stunDuration, true, collision.contacts[0].normal);
                }
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

            if (mSpeed != movementSpeed)
            {
                SetSpeed(mSpeed);
            }

			currentSpeed = cSpeed;
			netPosition = position;
			transform.rotation = rotation;
		}
	}
	

	public void Stunned(float duration, bool wall, Vector3 normal)
	{
		//stun stuff here
        state = PlayerState.STUNNED;
        Invoke("MakeAlive", duration);
        transform.forward = Vector3.Reflect(transform.forward, normal);


        rigidbody.AddExplosionForce(750, transform.position - transform.forward * 2, 0, 0);
        networkView.RPC("PlayStunnedFX", RPCMode.All, wall);
        SetSpeed(startSpeed);
        currentSpeed = 0;
        //ActivateEffects(false);
	}

    /* Check for players within a radius */
    public bool CheckNearbyPlayers(float mRadius)
    {
        bool returnValue = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, mRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.tag == "Player" && c.gameObject != gameObject)
            {
                PlayerController otherPlayer = c.gameObject.GetComponent<PlayerController>();
                if(this.movementSpeed >= otherPlayer.movementSpeed && otherPlayer.state != PlayerState.DEAD)
                    returnValue = true;
            }

        }

        return returnValue;
    }

    void SpawnSplatDecal(Vector3 dir)
    {
        GameObject decal = Instantiate(splatDecal, transform.position + (transform.up + transform.forward) * 0.5f, Quaternion.LookRotation(dir)) as GameObject;
        if (dir != Vector3.up &&
            dir != Vector3.down)
        {
            decal.transform.Rotate(45, 0, Random.RandomRange(0f, 360f));
        }

        else
        {
            decal.transform.Rotate(0, 0, Random.RandomRange(0f, 360f));
        }
    }

    void MakeAlive()
    {
        state = PlayerState.ALIVE;
    }

    void ActivateEffects(bool mActivated) {
        
        Debug.Log("set activated");
        foreach (AlphaMaterial am in GetComponentsInChildren<AlphaMaterial>())
        {
            am.SetActivated(mActivated);
        }
    }

	void Firstblood()
	{
		firstblood = true;

        if (Network.isServer)
        {
            networkView.RPC("PlayFirstBlood", RPCMode.All);
        }
	}

	[RPC]
	void ForcePosition(Vector3 pos)
	{
		transform.position = pos;
		HideModel (false);
		ShowRagdoll (false, false);

        animator.SetBool("idle", true);
        nameText.GetComponent<PlayerNameScript>().Reset();
	}

	[RPC]
	void PlayStunnedFX(bool wall)
	{
		if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
		{
			ScreenShaker.instance.Shake (1, 0.5f);
		}
        
		if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
		{
            if (!wall)
            {
                SoundStore.instance.PlayRandom(SoundStore.instance.StunSoundPlayer);
                SoundStore.instance.PlayRandom(SoundStore.instance.StunShout);
            }
        }
        if (wall)
            SoundStore.instance.PlayRandom(SoundStore.instance.StunSoundWall);
        
	}

	[RPC]
	void PlayFirstBlood()
	{ 
		SoundStore.instance.Play (SoundStore.instance.AnnouncerFirstBlood);
        ScoreBoard.instance.Display(Announcments.FIRST_BLOOD, 1f);
	}

	[RPC]
	void PlayDeathShout()
	{
		SoundStore.instance.PlayRandom (SoundStore.instance.DeathShout);
	}

	[RPC]
	void PlayWinSound(int killerId)
	{
        //This seems like as good a place as any to play a confetti particle system!
        confetti.particleSystem.Play();
		SoundStore.instance.Play (SoundStore.instance.WinSound);
        Connections.GetInstance().players[killerId].wins++;
	}


	[RPC]
	void Kill(int killerId, Vector3 dir, bool overkill, bool wall)
	{
        SetSpeed(startSpeed);
        SpawnSplatDecal(-dir);

        CancelInvoke();
        state = PlayerState.DEAD;

		HideModel (true);
		ShowRagdoll (true, overkill);
		splat.particleSystem.Play ();

		Debug.Log (playerInfo.id + " got killed by " + killerId);

        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");

        playerInfo.deaths++;
		if (overkill) {
			if (wall)
				SoundStore.instance.PlayRandom(SoundStore.instance.KillWall);
			else
				SoundStore.instance.PlayRandom(SoundStore.instance.OverkillSound);
		}
		else {
        	SoundStore.instance.PlayRandom(SoundStore.instance.KillSound);
		}
        if (Network.isServer &&
            GameController.instance.playersAlive == 2)
        {
            int winnerId = killerId;
            if (wall)
            {
                //find the winner 
                List<GameObject> alivePlayersList = GameController.instance.GetAlivePlayers();
                if (alivePlayersList.Count < 2)
                    winnerId = GameController.instance.GetAlivePlayers().ToArray()[0].GetComponent<PlayerController>().playerInfo.id;
            }

            networkView.RPC("PlayWinSound", RPCMode.All, winnerId);
        }

        //If you killed yourself, none of this applies
        if (playerInfo.id != killerId)
        {
            //Connections.GetInstance().players[killerId].killstreaks.Died();
			playerInfo.killstreaks.Died ();

            if (GameController.instance.playersAlive == Connections.GetInstance().players.Count && !firstblood)
            {
                Firstblood();
            }

            //You can get killerId by killerGameObject.GetComponent<PlayerController>().playerInfo.id
            Connections.GetInstance().players[killerId].kills++;

            Killstreaks killerKillstreak = Connections.GetInstance().players[killerId].killstreaks;
            killerKillstreak.AddKill();
            SoundStore.instance.PlayRandom(SoundStore.instance.KillShout);

            if (killerKillstreak.GetFastKills() == 2)
            {
                SoundStore.instance.Play(SoundStore.instance.AnnouncerDoubleKill);
                ScoreBoard.instance.Display(Announcments.DOUBLE_KILL, 1f);
            }
            else if (killerKillstreak.GetFastKills() == 3)
            {
                SoundStore.instance.Play(SoundStore.instance.AnnouncerMultiKill);
                ScoreBoard.instance.Display(Announcments.BRING_HONOR, 1f);
            }
            else if (killerKillstreak.GetKills() == 3)
            {
                SoundStore.instance.Play(SoundStore.instance.AnnouncerThreeKills);
                ScoreBoard.instance.Display(Announcments.MANY_KILL, 1f);
            }
            else if (killerKillstreak.GetKills() == 5)
            {
                SoundStore.instance.Play(SoundStore.instance.AnnouncerFiveKills);
                ScoreBoard.instance.Display(Announcments.LOTS_OF_KILL_LITTLE_TIME, 1f);
            }
            else if (killerKillstreak.GetKills() == 7)
            {
                SoundStore.instance.Play(SoundStore.instance.AnnouncerSevenKills);
                ScoreBoard.instance.Display(Announcments.GOJIRA_KILL, 1f);
            }

            //Bobber.instance.startClimax(1, 1);

        }

        else
        {
            //Bobber.instance.startClimax(0.5f, 0.5f);
        }

		if(Connections.GetInstance().localPlayers.Exists(x => x == playerInfo))
		{
			ScreenShaker.instance.Shake (1, 1);
            ScoreBoard.instance.spawnAmbulance();
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
		nameText.GetComponent<PlayerNameScript>().SetName(playerInfo.name, Connections.GetInstance().localPlayers.Exists(x => x.id == id));

        if (model != null)
        {
            foreach (Transform t in model.transform)
            {
                if (t.renderer != null)
                {
                    t.renderer.material = materials[playerInfo.id];
                    Debug.Log("Changed material to " + materials[playerInfo.id].name);
                }
            }
        }
	}
}
