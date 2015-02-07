using UnityEngine;
using System.Collections;

public class LobbyMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (!Connections.GetInstance ().isConnected) 
		{
			GUILayout.BeginHorizontal ();

			if(GUILayout.Button ("Host"))
			{
				Connections.GetInstance().HostLobby(8);
			}

			GUILayout.Button ("Refresh");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Nickname");
			Connections.GetInstance ().localNickname = GUILayout.TextField (Connections.GetInstance ().localNickname, GUILayout.Width (300));
			GUILayout.EndHorizontal ();
		}

		else
		{
			foreach(PlayerInfo p in Connections.GetInstance().players.Values)
			{
				GUILayout.Box(p.name + " | ID: " + p.id + " | Ready: " + p.ready);
			}
		}
	}
}