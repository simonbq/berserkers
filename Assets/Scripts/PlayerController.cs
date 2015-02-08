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

    public Animator animator;

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

        if (CheckNearbyPlayers(2.0f))
        {
            animator.SetBool("enemyclose", true);
        }
        else
        {
            animator.SetBool("enemyclose", false);
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
                animator.SetTrigger("attack");
                Debug.Log("Set trigger attack");

                PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
                if (hitPlayer.movementSpeed > this.movementSpeed)
                {
					
					Debug.Log ("Kill player");
					state = PlayerState.DEAD;
					rigidbody.AddExplosionForce(2000, transform.position + transform.forward * 2, 0, 0);

                    networkView.RPC("Kill", RPCMode.All, hitPlayer.playerInfo.id);

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
                }

                if (hitPlayer.movementSpeed == this.movementSpeed &&
				    state != PlayerState.STUNNED)
                {
                    Stunned(stunDuration);
                }
            }

            if (collision.gameObject.tag == "Wall" &&
			    state != PlayerState.STUNNED)
            {
                Stunned(stunDuration);
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
	
	void Stunned(float duration)
	{
		//stun stuff here
		state = PlayerState.STUNNED;
        Invoke("MakeAlive", duration);
        transform.Rotate(new Vector3(0, 180, 0));
		networkView.RPC ("PlayStunnedFX", RPCMode.All);
	}

    /* Check for players within a radius */
    public bool CheckNearbyPlayers(float mRadius)
    {
        bool returnValue = false;
        if (Physics.CheckSphere(transform.position, mRadius, (1 << LayerMask.NameToLayer("Player"))))
        {
            returnValue = true;
        }
        return returnValue;
    }

    void MakeAlive()
    {
        state = PlayerState.ALIVE;
    }

	[RPC]
	void PlayStunnedFX()
	{
		if (Connections.GetInstance ().playerId == playerInfo.id) 
		{
			//ScreenShaker.instance.Shake (1, 1);
		}

	}

	[RPC]
	void Kill(int killerId)
	{
		//You can get killerId by killerGameObject.GetComponent<PlayerController>().playerInfo.id
		Connections.GetInstance ().players [killerId].kills++;
		playerInfo.deaths++;

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
