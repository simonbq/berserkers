using UnityEngine;
using System.Collections;

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
