using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScoreBoard : MonoBehaviour {
	public static readonly Vector2 SCREEN_SIZE = new Vector2 (1920, 1080);
	
	public Rect area;
	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();
	public Texture statics;
	public Texture speedometer_bg;
	public Texture[] playerIcons = new Texture[8];
	public Material speedometer_mat;
	private static readonly Rect BACKGROUND_AREA = new Rect (0, 0, 1920, 1080);
	public Rect speedometer_rect = new Rect(0, 0, 512, 512);

	public Texture nosmoke_tex;
	public Rect nosmoke_rect = new Rect(0, 0, 0, 0);
	// Use this for initialization
	void Awake () {
		//speedometer_rect = new Rect (SCREEN_SIZE.x-600, 0, 600, 400);
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
		//GUI.DrawTexture (BACKGROUND_AREA, statics);
		if(Input.GetKey(KeyCode.Tab))
		{
			GUILayout.BeginArea (area, GUI.skin.box);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name"); 
			GUILayout.Label ("Kills");
			GUILayout.Label ("Deaths");
			GUILayout.EndHorizontal ();

			PlayerInfo[] players = new PlayerInfo[Connections.GetInstance().players.Values.Count];
			Connections.GetInstance().players.Values.CopyTo(players, 0);
			List<PlayerInfo> playas = new List<PlayerInfo>(players);
			playas.Sort();
			foreach(PlayerInfo p in playas)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Box(playerIcons[p.id]);
				GUILayout.Label (p.name); // Player name
				GUILayout.Label (p.kills.ToString()); // Player score
				GUILayout.Label (p.deaths.ToString()); // Player deaths
				GUILayout.EndHorizontal ();
			}
			// }
			GUILayout.EndArea ();
		}
		float f = 1.0f;
		if(HUDSingleton.instance.onFire) {
			f = Random.Range (0.9f, 1.1f);
		}
		speedometer_mat.SetFloat ("_Cutoff", HUDSingleton.instance.speed / 0.5f);
		if(Event.current.rawType == EventType.repaint)
			Graphics.DrawTexture (speedometer_rect, speedometer_mat.mainTexture, speedometer_mat);
		GUIUtility.ScaleAroundPivot (Vector2.one * f, nosmoke_rect.center);
		GUI.DrawTexture (nosmoke_rect, nosmoke_tex);
		Debug.Log (nosmoke_rect.center);
		GUIUtility.ScaleAroundPivot (-Vector2.one * f, nosmoke_rect.center);
	}
}
