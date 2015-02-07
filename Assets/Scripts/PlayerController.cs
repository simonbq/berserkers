using UnityEngine;
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
			if(input != value)
			{
				networkView.RPC ("UpdateInput", RPCMode.Server, value);
			}
		}
	}

	// Use this for initialization
	void Start () {
		state = PlayerState.ALIVE;

		renderer.material.color = playerColor;
		//Invoke ("Kill", 2);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Network.isServer)
		{
			if(state == PlayerState.ALIVE){
				rigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + _input * turnSpeed, transform.eulerAngles.z);
			}
		}

		else
		{
			if(playerInfo.networkPlayer == Network.player)
			{
				input = Input.GetAxis("Horizontal");
			}

			//do interpolation here Kappa DansGame
		}
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
		Vector3 velocity = new Vector3();
		Vector3 position = new Vector3();
		Quaternion rotation = Quaternion.identity; 

		if(stream.isWriting)
		{
			playerState = (int)playerState; 
			velocity = rigidbody.velocity;
			position = transform.position;
			rotation = transform.rotation;

			stream.Serialize(ref playerState);
			stream.Serialize(ref velocity);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		}

		else
		{
			stream.Serialize(ref playerState);
			stream.Serialize(ref velocity);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);

			state = (PlayerState)state;
			rigidbody.velocity = velocity;
			transform.position = position;
			transform.rotation = rotation;
		}
	}

	[RPC]
	void UpdateInput(float val)
	{
		_input = val;
	}
}
