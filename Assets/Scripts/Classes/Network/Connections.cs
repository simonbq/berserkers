using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInfo {
	public PlayerInfo(NetworkPlayer player, int pId, string nickname)
	{
		_name = nickname;
		_id = pId;
		_networkPlayer = player;
	}

	public bool ready = false;

	private NetworkPlayer _networkPlayer;
	private int _id;
	private string _name;

	public NetworkPlayer networkPlayer
	{
		get
		{
			return _networkPlayer;
		}
	}
	
	public string name
	{
		get
		{
			return _name;
		}
	}
	
	public int id
	{
		get
		{
			return _id;
		}
	}
}


public class Connections : MonoBehaviour {
	public string localNickname = "Unnamed";

	public bool isConnected
	{
		get
		{
			return _connected;
		}
	}

	public Dictionary<int, PlayerInfo> players
	{
		get
		{
			return _players;
		}
	}

	private bool _connected = false;
	private Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();
	private static Connections instance;

	public static Connections GetInstance()
	{
		if(instance == null)
		{
			Debug.LogError("There must be an instance of Connections in order to get the instance!");
		}

		return instance;
	}

	public void HostLobby(int maxPlayers)
	{
		Network.InitializeServer (maxPlayers, 61337, !Network.HavePublicAddress ());
		MasterServer.RegisterHost ("BerzerkasBananas", localNickname + "'s lobby");
	}

	void Start()
	{
		GameObject.DontDestroyOnLoad (this.gameObject);
		instance = this;
	}

	void OnServerInitialized()
	{
		_players.Add (0, new PlayerInfo(Network.player, 0, localNickname));
		_connected = true;
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Connected!");
		networkView.RPC ("SendPlayerInfo", RPCMode.Server, Network.player, localNickname);
		_connected = true;
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log ("Failed to connect! " + error);
	}

	[RPC]
	void PlayerConnected(NetworkPlayer player, int id, string nickname)
	{
		_players.Add (id, new PlayerInfo (player, id, nickname));
	}

	[RPC]
	void SendPlayerInfo(NetworkPlayer player, string nickname)
	{
		if (Network.isServer) 
		{
			int pId = players.Count;
			PlayerInfo connected = new PlayerInfo(player, pId, nickname);

			foreach(PlayerInfo p in players.Values)
			{
				networkView.RPC("PlayerConnected", player, p.networkPlayer, p.id, p.name);
			}

			_players.Add (pId, connected);
			networkView.RPC("PlayerConnected", RPCMode.All, connected.networkPlayer, connected.id, connected.name);
		}

		else
		{
			Debug.LogError("This should only go to the server!");
		}
	}
}
