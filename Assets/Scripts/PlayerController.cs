﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public enum PlayerState { ALIVE = 0, DEAD, STUNNED };
	public PlayerState state;
	public PlayerInfo playerInfo;

	//Player variables
    public float movementSpeed;
	public float turnSpeed;
	public Color playerColor;
    public float stunDuration;

	private float _input = 0;

	private bool firstblood = false;

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
	public void Start () {
		state = PlayerState.STUNNED;
        Invoke("MakeAlive", 2.0f);

		renderer.material.color = playerColor;
		

	}

    void Update()
    {
        if (!playerInfo.connected)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
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
            if (state == PlayerState.ALIVE &&
			    state != PlayerState.STUNNED)
            {
                rigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

                transform.Rotate(Vector3.up, _input * turnSpeed);
            }
        }

        else if(state != PlayerState.STUNNED &&
		        state != PlayerState.DEAD)
        {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.eulerAngles + Vector3.up * turnSpeed * _input), turnSpeed);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * movementSpeed, movementSpeed);
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
                PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
                if (hitPlayer.movementSpeed > this.movementSpeed)
                {
					
					Debug.Log ("Kill player");
					state = PlayerState.DEAD;
					SoundStore.instance.PlayRandom (SoundStore.instance.DeathShout);
					

					rigidbody.AddExplosionForce(2000, transform.position + transform.forward * 2, 0, 0);

					networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id);

					if (this.playerInfo.killstreaks.GetKills() == 0) {
						networkView.RPC ("PlayNoKill", RPCMode.All);
					}
					this.playerInfo.killstreaks.Died();

					int playersAlive = 0;
					foreach (GameObject player in GameController.instance.players)
					{
						if (player.GetComponent<PlayerController>().state == PlayerState.ALIVE)
							playersAlive++;
					}
					if (playersAlive < 2)
					{
						GameController.instance.Invoke("SpawnPlayers", 3);
					}
					if (playersAlive == Connections.GetInstance().players.Count - 1 && !firstblood) {
						Firstblood();
					}
                }
                else if (hitPlayer.movementSpeed == this.movementSpeed &&
				         state != PlayerState.STUNNED) {
					Stunned(stunDuration, false);
                }
            }

            if (collision.gameObject.tag == "Wall" &&
			    state != PlayerState.STUNNED)
				SoundStore.instance.PlayRandom (SoundStore.instance.StunSoundWall);
            {
                Stunned(stunDuration, true);
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
		Vector3 position = new Vector3();
		Vector3 velocity = new Vector3();
		Quaternion rotation = Quaternion.identity; 

		if(stream.isWriting)
		{
			playerState = (int)state;
			mSpeed = movementSpeed;
			position = transform.position;
			velocity = rigidbody.velocity;
			rotation = transform.rotation;

			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref rotation);
		}

		else
		{
			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref rotation);

			state = (PlayerState)playerState;
			if(state == PlayerState.DEAD)
			{
				rigidbody.velocity = velocity;
			}
			movementSpeed = mSpeed;
			transform.position = position;
			transform.rotation = rotation;
		}
	}
	
	void Stunned(float duration, bool wall)
	{
		//stun stuff here
		state = PlayerState.STUNNED;
        Invoke("MakeAlive", duration);
        transform.Rotate(new Vector3(0, 180, 0));
		networkView.RPC ("PlayStunnedFX", RPCMode.All, wall);
	}
    void MakeAlive()
    {
        state = PlayerState.ALIVE;
    }

	void Firstblood()
	{
		firstblood = true;
		networkView.RPC ("PlayFirstBlood", RPCMode.All);
	}

	[RPC]
	void PlayStunnedFX(bool wall)
	{
		if (Connections.GetInstance ().playerId == playerInfo.id) 
		{
			ScreenShaker.instance.Shake (1, 1);
		}
		if (wall)
			SoundStore.instance.PlayRandom (SoundStore.instance.StunSoundWall);
		else 
			SoundStore.instance.PlayRandom (SoundStore.instance.StunSoundPlayer);
		SoundStore.instance.PlayRandom (SoundStore.instance.StunShout);
	}

	[RPC]
	void PlayNoKill()
	{
		SoundStore.instance.Play(SoundStore.instance.AnnouncerNoKill);
	}

	[RPC]
	void PlayFirstBlood()
	{
		SoundStore.instance.Play (SoundStore.instance.AnnouncerFirstBlood);
	}

	[RPC]
	void Kill(int killerId)
	{
		//You can get killerId by killerGameObject.GetComponent<PlayerController>().playerInfo.id
		Connections.GetInstance ().players [killerId].kills++;
		playerInfo.deaths++;
		SoundStore.instance.PlayRandom (SoundStore.instance.KillSound);
		SoundStore.instance.PlayRandom (SoundStore.instance.KillShout);

		Killstreaks killerKillstreak = Connections.GetInstance ().players [killerId].killstreaks;

		killerKillstreak.AddKill ();
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

		if (Connections.GetInstance ().playerId == playerInfo.id) 
		{
			ScreenShaker.instance.Shake (1, 1);
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
