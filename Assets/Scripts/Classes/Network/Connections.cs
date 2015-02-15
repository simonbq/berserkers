using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerInfo : IComparable<PlayerInfo> {
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
	public string input = "Horizontal";

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

	public int CompareTo(PlayerInfo comparePart)
	{
		// A null value means that this object is greater. 
		if (comparePart == null)
			return 1;
		
		else 
			return this.id.CompareTo(comparePart.id);
	}
}


public class Connections : MonoBehaviour {
	public string localNickname = "Unnamed";
	public string lobbyScene = "Lobby";
	public Color[] playerColors;

	/*public int playerId
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
	}*/

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

	public List<PlayerInfo> localPlayers
	{
		get
		{
			return _localPlayers;
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

    public HostData[] serverList
    {
        get
        {
            return _servers;
        }
    }

    private HostData[] _servers;
	//private int _playerId = 0;
	//private PlayerInfo _playerInfo;
	private bool _connected = false;
	private bool _started = false;
	private Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();
	private Dictionary<NetworkPlayer, List<PlayerInfo>> _localPlayerCount = new Dictionary<NetworkPlayer, List<PlayerInfo>>();
	private List<PlayerInfo> _localPlayers = new List<PlayerInfo>();
	private static Connections instance;
	public static int port = 61337;

	public static Connections GetInstance()
	{
		return instance;
	}

	public void ToggleReady()
	{
		bool ready = !_localPlayers [0].ready;
		foreach(PlayerInfo player in _localPlayers)
		{
			player.ready = ready;
			networkView.RPC ("Ready", RPCMode.All, player.id, player.ready);
		}
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

    public void RefreshHostList()
    {
        MasterServer.RequestHostList("BerzerkasBananas");
    }

    public void AddLocalPlayer()
    {
		if (!Network.isServer)
        {
            networkView.RPC("SendPlayerInfo", RPCMode.Server, Network.player, localNickname, true);
        }

        else
        {
            SendPlayerInfo(Network.player, localNickname, true);
        }
    }

	public void KickLocalPlayer()
	{
		if(_localPlayers.Count > 1)
		{
			networkView.RPC ("PlayerDisconnected", RPCMode.All, Network.player, _localPlayers[_localPlayers.Count - 1].id, true);
		}
	}
	
	void Awake() {
		localNickname = Prefs.GetName ();
	}


	void Start()
	{
		GameObject.DontDestroyOnLoad (this.gameObject);

        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

		instance = this;

        MasterServer.ClearHostList();
        RefreshHostList();
	}

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            _servers = MasterServer.PollHostList();
        }
    }

	void OnDisconnectedFromServer()
	{
        if (Application.loadedLevelName != lobbyScene)
        {
            Application.LoadLevel(lobbyScene);
            Destroy(this.gameObject);
        }

        else
        {
            _started = false;
            _connected = false;
            players.Clear();
			_localPlayers.Clear();
        }
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
		if(_localPlayerCount.ContainsKey(player))
		{
			var disconnected = _localPlayerCount[player];

			foreach(var d in disconnected)
			{
				networkView.RPC ("PlayerDisconnected", RPCMode.Others, player, d.id, false);
				if(_players.ContainsKey (d.id))
				{
					_players[d.id].connected = false;
					_players.Remove(d.id);
				}

				Debug.Log ("Removing player " + d.id);
			}

			_localPlayerCount.Remove (player);
		}

		else
		{
			int id = GetPlayerId(player);
			networkView.RPC ("PlayerDisconnected", RPCMode.All, player, id, false);
			players[id].connected = false;
			_players.Remove(id);
		}
	}

	void OnServerInitialized()
	{
		 PlayerInfo playerInfo = new PlayerInfo (Network.player, 0, localNickname, true);
		_players.Add (0, playerInfo);
		_localPlayers.Add (playerInfo);
		_localPlayerCount.Add (Network.player, new List<PlayerInfo>());
		_localPlayerCount [Network.player].Add (playerInfo);
		_connected = true;
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Connected!");
		networkView.RPC ("SendPlayerInfo", RPCMode.Server, Network.player, localNickname, false);
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
		Debug.Log ("Name = " + localNickname);
		Prefs.SetName (localNickname);
		Prefs.SetVolume (AudioListener.volume);
		Prefs.Save ();
		Application.LoadLevel (name);
		Debug.Log ("Going to Scene " + name);

		_started = true;
	}

	[RPC]
	void PlayerDisconnected(NetworkPlayer player, int id, bool local)
	{
		if(local)
		{
			if(Network.isServer)
			{
				_localPlayerCount[player].RemoveAll(x => x.id == id);
				Debug.Log ("A client removed a local player");
			}
			
			if(_localPlayers.Exists (x => x.id == id))
			{
				_localPlayers.Remove (_localPlayers.Find (x => x.id == id));
				Debug.Log ("Removed local player");
			}
		}

		_players[id].connected = false;
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
            //_playerId = id;
            //_playerInfo = connected;
			if(localPlayers.Count >= 1)
			{
				connected.input = "Horizontal P" + (localPlayers.Count + 1);
			}

			_localPlayers.Add (connected);
            _connected = true;
        }
	}

	[RPC]
	void SendPlayerInfo(NetworkPlayer player, string nickname, bool local)
	{
		if (Network.isServer) 
		{
			if(_players.Count >= Network.maxConnections)
			{
				if(!_localPlayerCount.ContainsKey (player))
				{
					Network.CloseConnection (player, true);
				}
			}

			else
			{
				if(!_localPlayerCount.ContainsKey(player))
				{
					_localPlayerCount.Add(player, new List<PlayerInfo>());
				}

				if(_localPlayerCount[player].Count < 4)
				{
					int cId = GetPlayerId(player);
		            int pId = players.Count;

					if(local)
					{
						nickname = nickname + "(" + (_localPlayerCount[player].Count + 1) + ")";
					}

		            PlayerInfo connected = new PlayerInfo(player, pId, nickname, false);
					_localPlayerCount[player].Add (connected);

		            if (!local)
		            {
		                foreach (PlayerInfo p in players.Values)
		                {
		                    networkView.RPC("PlayerConnected", player, p.networkPlayer, p.id, p.name, p.ready);
		                }
		            }

		            else
		            {
						connected.ready = true;
		            }

					networkView.RPC("PlayerConnected", RPCMode.All, connected.networkPlayer, connected.id, connected.name, connected.ready);
				}
			}
		}

		else
		{
			Debug.LogError("This should only go to the server!");
		}
	}
}
