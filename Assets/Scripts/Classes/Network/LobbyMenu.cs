using UnityEngine;
using System.Collections;

public class LobbyMenu : MonoBehaviour {

	string ipNumber = "127.0.0.1";

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

			GUILayout.BeginHorizontal ();
			GUILayout.Label("IP");
			ipNumber = GUILayout.TextField (ipNumber, GUILayout.Width (300));
			GUILayout.EndHorizontal();

			if(GUILayout.Button ("Connect"))
			{
				Network.Connect (ipNumber, 61337);
				Debug.Log ("Connecting to " + ipNumber);
			}
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