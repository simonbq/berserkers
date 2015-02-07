using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private static GameController _instance;
	public static GameController instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<GameController>();
				DontDestroyOnLoad(_instance.gameObject);
			}
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

	void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
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
				players.Add (SpawnPlayer(spawnPoints, player.id));
			}

			SpawnPowerUp(spawnPoints);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject SpawnPlayer(List<GameObject> mSpawnPoints, int id){
		GameObject selectSpawnPoint = mSpawnPoints[Random.Range(0, mSpawnPoints.Count)];
		//Debug.Log ("Spawned player at "+selectSpawnPoint.transform.position);
		GameObject player = Network.Instantiate(playerPrefab, selectSpawnPoint.transform.position + new Vector3(0, 2, 0), selectSpawnPoint.transform.rotation, id) as GameObject;
		player.GetComponent<PlayerController> ().SetPlayer (id);
		player.name = "ID_" + id;
		return player;
	}

	public GameObject SpawnPowerUp(List<GameObject> mSpawnPoints){
		GameObject selectSpawnPoint = mSpawnPoints[Random.Range(0, mSpawnPoints.Count)];



		//Debug.Log ("Spawned powerup at "+selectSpawnPoint.transform.position);

		return Instantiate(powerupPrefab, selectSpawnPoint.transform.position + new Vector3(0, 2, 0), selectSpawnPoint.transform.rotation) as GameObject;
	}
}
