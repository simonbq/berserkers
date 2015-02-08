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

	public enum GameState { INGAME };
	public static GameState state;

	public List<GameObject> spawnPoints;

	public List<GameObject> players;

	//Prefabs
	public GameObject playerPrefab;
	public GameObject powerupPrefab;
	public float powerupSpawnTime = 5f;

	private bool powerupSpawned = true;

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
		if(powerupSpawned)
		{
			Invoke ("SpawnPowerUp", powerupSpawnTime);
			powerupSpawned = false;
		}
	}

    public void SpawnPlayers()
    {
		GameObject[] powerups = GameObject.FindGameObjectsWithTag ("Powerup");
		foreach(GameObject powerup in powerups)
		{
			Network.Destroy(powerup);
		}

		if (players.Count == 0)
        {
			foreach (PlayerInfo player in Connections.GetInstance().players.Values)
            {
				Debug.Log ("Spawning player " + player.name);
				GameObject playerObject = SpawnPlayer(player.id);

				if(playerObject != null)
				{
					players.Add (playerObject);
				}
			}
		}
        else
        {
            foreach (GameObject player in players)
            {
				GameObject selectSpawnPoint = GetSpawn();

				if(selectSpawnPoint != null)
				{
	                player.transform.position =  selectSpawnPoint.transform.position + new Vector3(0, 2, 0);
	                player.transform.rotation = selectSpawnPoint.transform.rotation;

	                PlayerController pc = player.GetComponent<PlayerController>();
	                pc.Reset();
				}
            }
        }
        
    }

	GameObject SpawnPlayer(int id){
		GameObject selectSpawnPoint = GetSpawn();
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
		GameObject selectSpawnPoint = GetSpawn();

		if(selectSpawnPoint != null)
		{
			Network.Instantiate(powerupPrefab, selectSpawnPoint.transform.position + new Vector3(0, 0.01f, 0), selectSpawnPoint.transform.rotation, 0);

			powerupSpawned = true;
		}
	}

	GameObject GetSpawn()
	{
		List<GameObject> visited = new List<GameObject> ();

		do
		{
			List<GameObject> remaining = spawnPoints.Except(visited).ToList();
			GameObject selected = remaining[Random.Range (0, remaining.Count)];

			if(!Physics.CheckSphere(selected.transform.position + Vector3.up, 1.5f, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Powerup")))
			{
				//Debug.Log ("Found spawnpoint!");
				return selected;
			}

			if(!visited.Exists(x => x == selected))
			{
				visited.Add (selected);
			}

		}
		while(visited.Count != spawnPoints.Count);

		Debug.Log ("Failed to find suitable spawnpoint");
		return null;
	}
}
