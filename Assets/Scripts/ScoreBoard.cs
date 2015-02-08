﻿using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {
	public static readonly Vector2 SCREEN_SIZE = new Vector2 (1920, 1080);
	
	public Rect area;
	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();
	public Texture statics;
	public Texture speedometer_bg;
	public Material speedometer_mat;
	private static readonly Rect BACKGROUND_AREA = new Rect (0, 0, 1920, 1080);
	private Rect speedometer_rect;
	// Use this for initialization
	void Awake () {
		speedometer_rect = new Rect (0, 0, 512, 512);
	}
	
	// Update is called once per frame
	void Update () {
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));
	}
	
	void OnGUI() {
		GUI.matrix = matrix;
		if(skin != null) {
			GUI.skin = skin;
		}
		GUI.DrawTexture (BACKGROUND_AREA, statics);
		if(Input.GetKey(KeyCode.Tab))
		{
			GUILayout.BeginArea (area, GUI.skin.box);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name"); 
			GUILayout.Label ("Kills");
			GUILayout.Label ("Deaths");
			GUILayout.EndHorizontal ();

			foreach(PlayerInfo p in Connections.GetInstance().players.Values)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label (p.name); // Player name
				GUILayout.Label (p.kills.ToString()); // Player score
				GUILayout.Label (p.deaths.ToString()); // Player deaths
				GUILayout.EndHorizontal ();
			}
			// }
			GUILayout.EndArea ();	
		}
		speedometer_mat.SetFloat ("cutoff", Random.Range(0.0f, 1.0f));

		//GUI.DrawTexture (speedometer_rect, speedometer_bg);
		Graphics.DrawTexture (speedometer_rect, speedometer_mat.mainTexture, speedometer_mat);
	}
}
