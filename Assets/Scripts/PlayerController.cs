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
	private Vector3 netPosition = new Vector3();

	private float input
	{
		set
		{
			if(_input != value)
			{
				networkView.RPC ("UpdateInput", RPCMode.All, value);
			}
		}
	}

	public int playerIDD;
	// Use this for initialization
	void Start () {
		state = PlayerState.ALIVE;

		renderer.material.color = playerColor;
		//Invoke ("Kill", 2);

		if(!Network.isServer)
		{
			rigidbody.isKinematic = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(playerInfo.id == Connections.GetInstance().playerId)
		{
			input = Input.GetAxis("Horizontal");
		}

		if(Network.isServer)
		{
			if(state == PlayerState.ALIVE){
				//Debug.Log ("Input is for player " + playerInfo.name + " is " + _input);
				rigidbody.MovePosition(transform.position + transform.forward * movementSpeed);

				transform.Rotate (Vector3.up, _input * turnSpeed);
			}
		}

		else
		{
			float dist = Vector3.Distance(transform.position, netPosition);
			float localSpeed = Mathf.Max(movementSpeed, dist);
			Vector3 localPos = Vector3.Lerp(transform.position, netPosition, movementSpeed * Time.fixedDeltaTime);
			Vector3 targetPos = localPos + transform.rotation.eulerAngles * localSpeed;

			transform.position = Vector3.MoveTowards(localPos, targetPos, localSpeed * Time.fixedDeltaTime);

			//Quaternion predict = Quaternion.FromToRotation(transform.eulerAngles, new Vector3(0, _input, 0));
			//transform.rotation = Quaternion.Lerp (transform.rotation, predict, Time.fixedDeltaTime);
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
			playerState = (int)playerState; 
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

			state = (PlayerState)state;
			movementSpeed = mSpeed;
			netPosition = position;
			transform.rotation = rotation;
		}
	}

	[RPC]
	void UpdateInput(float val)
	{
		_input = val;
		//Debug.Log ("Got update " + val);
	}

	[RPC]
	void Spawned(int id)
	{
		playerInfo = Connections.GetInstance ().players [id];
	}
}
