using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

			foreach(PlayerInfo player in Connections.GetInstance().players.Values)
			{
				Debug.Log ("Spawning player " + player.name);
				players.Add (SpawnPlayer(player.id));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(powerupSpawned)
		{
			Invoke ("SpawnPowerUp", powerupSpawnTime);
			powerupSpawned = false;
		}
	}

	GameObject SpawnPlayer(int id){
		GameObject selectSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
		//Debug.Log ("Spawned player at "+selectSpawnPoint.transform.position);
		GameObject player = Network.Instantiate(playerPrefab, selectSpawnPoint.transform.position + new Vector3(0, 2, 0), selectSpawnPoint.transform.rotation, id) as GameObject;
		player.GetComponent<PlayerController> ().SetPlayer (id);
		player.name = "ID_" + id;
		return player;
	}

	void SpawnPowerUp(){
		GameObject selectSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

		//Debug.Log ("Spawned powerup at "+selectSpawnPoint.transform.position);

		Network.Instantiate(powerupPrefab, selectSpawnPoint.transform.position + new Vector3(0, 0.5f, 0), selectSpawnPoint.transform.rotation, 0);

		powerupSpawned = true;
	}
}
