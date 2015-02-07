﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public enum PlayerState { ALIVE, DEAD };
	public PlayerState state;
	public PlayerInfo playerInfo;

	//Player variables
    public float movementSpeed;
	public float turnSpeed;
	public Color playerColor;

	public AudioClip[] audioClips;

	private float _input = 0;

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
	void Start () {
		state = PlayerState.ALIVE;

		renderer.material.color = playerColor;
		//Invoke ("Kill", 2);
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
            if (state == PlayerState.ALIVE)
            {
                rigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

                transform.Rotate(Vector3.up, _input * turnSpeed);
            }
        }

        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.eulerAngles + Vector3.up * turnSpeed * _input), turnSpeed);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * movementSpeed, movementSpeed);
        }
	}

	public void SetPlayer(int id)
	{
		networkView.RPC ("Spawned", RPCMode.All, id);
	}

	public void Kill(){
		Debug.Log ("Kill player");
		state = PlayerState.DEAD;
		renderer.material.color = Color.black;
		rigidbody.AddExplosionForce(2000, transform.position + transform.forward*2, 0, 0);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int playerState = 0;
		float mSpeed = 0;
		Vector3 position = new Vector3();
		Quaternion rotation = Quaternion.identity; 

		if(stream.isWriting)
		{
			playerState = (int)state;
			mSpeed = movementSpeed;
			position = transform.position;
			rotation = transform.rotation;

			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		}

		else
		{
			stream.Serialize(ref playerState);
			stream.Serialize(ref mSpeed);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);

			state = (PlayerState)playerState;
			movementSpeed = mSpeed;
			transform.position = position;
			transform.rotation = rotation;
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
