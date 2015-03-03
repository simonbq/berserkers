using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {

	private static GameController _instance;
	public static GameController instance
	{
		get
		{
			_instance = GameObject.FindObjectOfType<GameController>();
			return _instance;
		}
	}

	public enum GameState { INGAME, ROUNDEND };
	public GameState state;

	public List<GameObject> spawnPoints;
    private List<GameObject> occupiedSpawns = new List<GameObject>();
    private List<GameObject> powerupSpawns = new List<GameObject>();

	public List<GameObject> players;

    public int playersAlive = 0;

	//Prefabs
	public GameObject playerPrefab;
	public GameObject powerupPrefab;
	public float powerupSpawnTime = 5f;
    public float powerupRoundStart = 5f;

	private bool powerupSpawned = true;
    private bool spawnPowerups = false;
    private int countdown = 3;

    void StartSpawningPowerups()
    {
        spawnPowerups = true;
    }

	void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
		}
		else
		{
			if(this!=_instance)
				Destroy(this.gameObject);
		}
	}


	// Use this for initialization
	void Start () {
		if(Network.isServer)
		{
			state = GameState.INGAME;

			foreach(GameObject o in GameObject.FindGameObjectsWithTag("SpawnPoint")){
				spawnPoints.Add(o);
				//Debug.Log ("Found spawn point: "+o.transform.position);
			}

			SpawnPlayers();
		}
		SoundStore.instance.Play (SoundStore.instance.MatchStart);
	}
	
	// Update is called once per frame
	void Update () {

		if(powerupSpawned && spawnPowerups)
		{
			Invoke ("SpawnPowerUp", powerupSpawnTime);
			powerupSpawned = false;
		}

        List<GameObject> alivePlayersList = GetAlivePlayers();
        playersAlive = alivePlayersList.Count;

        if (Network.isServer &&
            GameController.instance.state != GameController.GameState.ROUNDEND)
        {
            /*if (countAlive != 0)
            {
                Bobber.instance.cheerFactor = 1 - (1 / countAlive);
            }*/
            if ((playersAlive < 2 && players.Count > 1) || (playersAlive < 1 && players.Count == 1))
            {
                GameController.instance.state = GameController.GameState.ROUNDEND;

                GameController.instance.Invoke("SpawnPlayers", 5);
                

            }
        }

		var disconnected = players.FindAll(x => !x.GetComponent<PlayerController>().playerInfo.connected);

		foreach(var d in disconnected)
		{
			players.Remove(d);
			Network.Destroy (d);
		}


	}

    public List<GameObject> GetAlivePlayers()
    {
        return players.FindAll(x => x.GetComponent<PlayerController>().state != PlayerController.PlayerState.DEAD);
    }

	void AnnouncerStart()
	{
		SoundStore.instance.Play (SoundStore.instance.AnnouncerStart);
		Debug.Log ("Play round start sound now");
	}

    public void SpawnPlayers()
    {
		Invoke ("AnnouncerStart", 2.0f);
        countdown = 3;
        //Countdown();
        state = GameState.INGAME;
        spawnPowerups = false;
        CancelInvoke("SpawnPowerUp");
        powerupSpawned = true;
        Invoke("StartSpawningPowerups", powerupRoundStart);
        powerupSpawns.Clear();

        GameObject[] powerups = GameObject.FindGameObjectsWithTag ("Powerup");
		foreach(GameObject powerup in powerups)
		{
			Network.Destroy(powerup);
		}

        occupiedSpawns.Clear();

		if (players.Count == 0)
        {
            foreach (PlayerInfo player in Connections.GetInstance().players.Values)
            {
                Debug.Log("Spawning player " + player.name);
                GameObject playerObject = SpawnPlayer(player.id);

                if (playerObject != null)
                {
                    players.Add(playerObject);
                }
            }
		}
        else
        {
            foreach (GameObject player in players)
            {
                GameObject selectSpawnPoint = GetSpawn(ref occupiedSpawns);

                if (selectSpawnPoint != null)
                {
                    player.transform.position = selectSpawnPoint.transform.position + new Vector3(0, 2, 0);
                    player.transform.rotation = selectSpawnPoint.transform.rotation;

                    PlayerController pc = player.GetComponent<PlayerController>();
                    pc.Reset();
                }
            }
        }
        
    }

	GameObject SpawnPlayer(int id){
		GameObject selectSpawnPoint = GetSpawn(ref occupiedSpawns);
		if(selectSpawnPoint != null)
		{
			//Debug.Log ("Spawned player at "+selectSpawnPoint.transform.position);
			GameObject player = Network.Instantiate(playerPrefab, selectSpawnPoint.transform.position + new Vector3(0, 2, 0), selectSpawnPoint.transform.rotation, id) as GameObject;
			player.GetComponent<PlayerController> ().SetPlayer (id);
			player.name = "ID_" + id;

			return player;
		}

		return null;
	}

	void SpawnPowerUp(){
        if (spawnPowerups && Application.loadedLevelName!="Lobby")
        {
            int spawnsLeft = spawnPoints.Count - powerupSpawns.Count;
            int alive = Mathf.CeilToInt((float)playersAlive / 2);
            Debug.Log(alive);
            
            for (int i = 0; i < Random.Range(1, Mathf.Min(spawnsLeft, alive + 1)); i++)
            {
                GameObject selectSpawnPoint = GetSpawn(ref powerupSpawns);

                if (selectSpawnPoint != null)
                {
                    GameObject spawned = Network.Instantiate(powerupPrefab, selectSpawnPoint.transform.position + new Vector3(0, 0.01f, 0), selectSpawnPoint.transform.rotation, 0) as GameObject;

                    if (Network.isServer)
                    {
                        spawned.GetComponent<PowerupScript>().spawnPoint = selectSpawnPoint;
                    }
                }
            }
        }

        powerupSpawned = true;
	}

	GameObject GetSpawn(ref List<GameObject> occupied)
	{
		List<GameObject> remaining = spawnPoints.Except(occupied).ToList();

        if (remaining.Count > 0)
        {
            GameObject selected = remaining[Random.Range(0, remaining.Count)];
            occupied.Add(selected);

            return selected;
        }

        Debug.Log("Failed to find a suitable spawn");
        return null;
	}

    public void ReleaseRuneSpawn(GameObject point)
    {
        if (powerupSpawns.Contains(point))
        {
            powerupSpawns.Remove(point);
        }
    }

    void Countdown()
    {
        switch (countdown)
        {
            case 3:
                Connections.GetInstance().DisplaySomething(Announcments.READY, 10f);
                break;
            case 2:
                Connections.GetInstance().DisplaySomething(Announcments.SET, 10f);
                break;
            case 1:
                Connections.GetInstance().DisplaySomething(Announcments.SOON, 10f);
                break;
            case 0:
                Connections.GetInstance().DisplaySomething(Announcments.GO, 1f);
                break;
        }

        if (countdown != 0)
        {
            countdown--;
            Invoke("Countdown", 1f);
        }
    }
}
