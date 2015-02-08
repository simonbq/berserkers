using UnityEngine;
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
            if (state == PlayerState.ALIVE &&
			    state != PlayerState.STUNNED)
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

    void OnCollisionEnter(Collision collision)
    {
        if (Network.isServer)
        {
            if (collision.gameObject.tag == "Player")
            {
                PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
                if (hitPlayer.movementSpeed > this.movementSpeed)
                {
                    networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id);
                }
                if (hitPlayer.movementSpeed == this.movementSpeed)
                {
                    networkView.RPC("Stunned", RPCMode.All, stunDuration);
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
	void Stunned(float duration)
	{
		//stun stuff here
		state = PlayerState.STUNNED;
        Invoke("MakeAlive", duration);
	}
    void MakeAlive()
    {
        state = PlayerState.ALIVE;
    }

	[RPC]
	void Kill(int killerId)
	{
		//You can get killerId by killerGameObject.GetComponent<PlayerController>().playerInfo.id
		Connections.GetInstance ().players [killerId].kills++;
		playerInfo.deaths++;

		Debug.Log ("Kill player");
		state = PlayerState.DEAD;
		renderer.material.color = Color.black;
		rigidbody.AddExplosionForce(2000, transform.position + transform.forward * 2, 0, 0);
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
