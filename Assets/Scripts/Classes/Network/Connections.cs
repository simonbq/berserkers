using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInfo {
	public PlayerInfo(NetworkPlayer player, int pId, string nickname, bool isReady)
	{
		_name = nickname;
		_id = pId;
		_networkPlayer = player;
		ready = isReady;
	}

	public bool ready = false;
    public bool connected = true;
	public int kills = 0;
	public int deaths = 0;

	public Killstreaks killstreaks = new Killstreaks ();

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
	public string lobbyScene = "Lobby";

	public int playerId
	{
		get
		{
			return _playerId;
		}
	}

	public PlayerInfo playerInfo
	{
		get
		{
			return _playerInfo;
		}
	}

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

	public bool playersReady
	{
		get
		{
			return !_players.Any (x => !x.Value.ready);
		}
	}

	public bool hasStarted
	{
		get
		{
			return _started;
		}
	}

	public NetworkPlayer server
	{
		get
		{
			return players[0].networkPlayer;
		}
	}

	private int _playerId = 0;
	private PlayerInfo _playerInfo;
	private bool _connected = false;
	private bool _started = false;
	private Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();
	private static Connections instance;
	public static int port = 61337;

	public static Connections GetInstance()
	{
		return instance;
	}

	public void ToggleReady()
	{
		playerInfo.ready = !playerInfo.ready;
		networkView.RPC ("Ready", RPCMode.All, playerId, playerInfo.ready);
	}

	public void HostLobby(int maxPlayers)
	{
		Network.InitializeServer (maxPlayers, port, !Network.HavePublicAddress ());
		MasterServer.RegisterHost ("BerzerkasBananas", localNickname + "'s lobby");
	}

	public void StartGame(string scene)
	{
		if(Network.isServer &&
		   !hasStarted)
		{
			if(playersReady)
			{
				networkView.RPC ("GotoScene", RPCMode.All, scene);
                MasterServer.UnregisterHost();
			}

			else
			{
				Debug.Log ("Players are not ready!!!");
			}
		}
	}

	void Start()
	{
		GameObject.DontDestroyOnLoad (this.gameObject);
		instance = this;
	}

	void OnDisconnectedFromServer()
	{
		Application.LoadLevel (lobbyScene);
		Destroy (this.gameObject);
	}

    void OnPlayerConnected(NetworkPlayer player)
    {
        if (_started)
        {
            Network.CloseConnection(player, true);
        }
    }

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		int id = GetPlayerId(player);
        players[id].connected = false;
        _players.Remove(id);
		Debug.Log ("ID: " + id);
		networkView.RPC ("PlayerDisconnected", RPCMode.All, id);
	}

	void OnServerInitialized()
	{
		_playerInfo = new PlayerInfo (Network.player, 0, localNickname, true);
		_players.Add (0, _playerInfo);
		_connected = true;
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Connected!");
		networkView.RPC ("SendPlayerInfo", RPCMode.Server, Network.player, localNickname);
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log ("Failed to connect! " + error);
	}

    int GetPlayerId(NetworkPlayer player)
    {
        if (_players.Any(x => x.Value.networkPlayer == player))
        {
            return _players.FirstOrDefault(x => x.Value.networkPlayer == player).Value.id;
        }

        return -1;
    }

	[RPC]
	void GotoScene(string name)
	{
		Application.LoadLevel (name);
		Debug.Log ("Going to Scene " + name);

		_started = true;
	}

	[RPC]
	void PlayerDisconnected(int id)
	{
		_players.Remove (id);
	}

	[RPC]
	void Ready(int id, bool ready)
	{
		players [id].ready = ready;
	}

	[RPC]
	void PlayerConnected(NetworkPlayer player, int id, string nickname, bool ready)
	{
        PlayerInfo connected = new PlayerInfo(player, id, nickname, ready);
        _players.Add(id, connected);

        if (Network.player == player)
        {
            _playerId = id;
            _playerInfo = connected;
            _connected = true;
        }
	}

	[RPC]
	void SendPlayerInfo(NetworkPlayer player, string nickname)
	{
		if (Network.isServer) 
		{
            int cId = GetPlayerId(player);
            int pId = players.Count;
            PlayerInfo connected = new PlayerInfo(player, pId, nickname, false);

            foreach (PlayerInfo p in players.Values)
            {
                networkView.RPC("PlayerConnected", player, p.networkPlayer, p.id, p.name, p.ready);
            }

			networkView.RPC("PlayerConnected", RPCMode.All, connected.networkPlayer, connected.id, connected.name, connected.ready);
		}

		else
		{
			Debug.LogError("This should only go to the server!");
		}
	}
}
