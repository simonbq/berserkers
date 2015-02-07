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
		state = GameState.INGAME;

		foreach(GameObject o in GameObject.FindGameObjectsWithTag("SpawnPoint")){
			spawnPoints.Add(o);
			Debug.Log ("Found spawn point: "+o.transform.position);
		}


		players.Add (SpawnPlayer(spawnPoints));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject SpawnPlayer(List<GameObject> mSpawnPoints){

		GameObject selectSpawnPoint = mSpawnPoints[Random.Range(0, mSpawnPoints.Count)];
		Debug.Log ("Spawned player at "+selectSpawnPoint.transform.position);

		return Instantiate(playerPrefab, selectSpawnPoint.transform.position + new Vector3(0, 2, 0), selectSpawnPoint.transform.rotation) as GameObject;
	}

	//public GameObject SpawnPickUp();
}
