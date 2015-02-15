using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Announcments {
	BRING_HONOR, DOUBLE_KILL, FIRST_BLOOD, GO, GOJIRA_KILL,
	LOTS_OF_KILL_LITTLE_TIME, MANY_KILL, READY, SET, SOON
}

public class ScoreBoard : MenuBase {
	public static ScoreBoard instance { get; set; }
	public Vector2 screen_scale { get { return screenScale; } }
	
	public Rect area;
	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();
	public Texture statics;
	public Texture speedometer_bg;
	public Texture[] playerIcons = new Texture[8];
	public Texture[] announcements = new Texture[10];
	private Texture currentAnnouncement = null;
	public Material speedometer_mat;
	private static readonly Rect BACKGROUND_AREA = new Rect (0, 0, 1920, 1080);
	public Rect speedometer_rect = new Rect(0, 0, 512, 512);
	public Rect speedometer_textrect = new Rect (0, 0, 512, 512);
	public Rect announcerRect = new Rect (800, 300, 256, 512);
	public Texture nosmoke_tex;
	public Rect nosmoke_rect = new Rect(0, 0, 0, 0);
	public float quote_maxSpeed_endquote = 9000.0f;
	private float est_maxSpeed = 0.6f;

	private bool menuActive = false;
	private float randomSpeed = 0;

	private List<PlayerInfo> playas;
	// Use this for initialization
	void Awake () {
		instance = this;
		setCurrent (new GameMenuState ());
		//speedometer_rect = new Rect (SCREEN_SIZE.x-600, 0, 600, 400);
	}
	
	// Update is called once per frame
	void Update () {
		//ScreenShaker.instance.SetConstantShakyness (Mathf.Pow(HUDSingleton.instance.speed, 4) );
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		screenScale = new Vector2 (width, height);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));

		PlayerInfo[] players = new PlayerInfo[Connections.GetInstance().players.Values.Count];
		Connections.GetInstance().players.Values.CopyTo(players, 0);
		playas = new List<PlayerInfo>(players);
		playas.Sort();
		est_maxSpeed = Mathf.Max (est_maxSpeed, HUDSingleton.instance.speed);
		if(Input.GetButtonUp("toggleMenu")) {
			menuActive = !menuActive;
		}
		current.update (this);
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
			GUIShadow.LayoutLabel ("Name"); 
			GUIShadow.LayoutLabel ("Kills");
			GUIShadow.LayoutLabel ("Deaths");
			GUILayout.EndHorizontal ();


			foreach(PlayerInfo p in playas)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Box(playerIcons[p.id], skin.customStyles[0]);
				GUI.color = Connections.GetInstance().playerColors[p.id];
				GUIShadow.LayoutLabel (p.name); // Player name
				GUI.color = Color.white;
				GUIShadow.LayoutLabel (p.kills.ToString()); // Player score
				GUIShadow.LayoutLabel (p.deaths.ToString()); // Player deaths
				GUILayout.EndHorizontal ();
			}
			// }
			GUILayout.EndArea ();
		}
		float f = 1.0f;
		if(HUDSingleton.instance.onFire) {
			f = Random.Range (0.9f, 1.1f);
		}

		if(Connections.GetInstance().localPlayers.Count == 1)
		{
			speedometer_mat.SetFloat ("_Cutoff", HUDSingleton.instance.speed / est_maxSpeed);
			if(Event.current.rawType == EventType.repaint)
				Graphics.DrawTexture (speedometer_rect, speedometer_mat.mainTexture, speedometer_mat);
			GUI.Label (speedometer_textrect, (int)(Mathf.Pow(HUDSingleton.instance.speed, 3)/Mathf.Pow(est_maxSpeed, 3) * (quote_maxSpeed_endquote + (float)Random.Range(0, 2))) + "", GUI.skin.customStyles [1]); //
		}

		else
		{
			randomSpeed = Mathf.Lerp (randomSpeed, Random.Range (0f, 1f), 3 * Time.deltaTime);
			speedometer_mat.SetFloat("_Cutoff", randomSpeed);

			if(Event.current.rawType == EventType.repaint)
				Graphics.DrawTexture (speedometer_rect, speedometer_mat.mainTexture, speedometer_mat);
			GUI.Label (speedometer_textrect, "N/A", GUI.skin.customStyles [1]); //
		}
		if(currentAnnouncement != null) {
			GUI.DrawTexture(announcerRect, currentAnnouncement);
		}

		if(HUDSingleton.instance.onFire && Options.dynamicHud) {
			GUIUtility.ScaleAroundPivot (Vector2.one * f, nosmoke_rect.center);
		}
		GUI.DrawTexture (nosmoke_rect, nosmoke_tex);
		if(HUDSingleton.instance.onFire && Options.dynamicHud) {
			GUIUtility.ScaleAroundPivot (Vector2.one * (1/f), nosmoke_rect.center);
		}
		if(menuActive) {
			current.render();
		}
	}

	private IEnumerator tempdisplay (int id, float duration) {
		Texture t = announcements [id];
		currentAnnouncement = t;
		yield return new WaitForSeconds (duration);
		if(currentAnnouncement == t)
			currentAnnouncement = null;
	}

	public void Display(Announcments announcment, float duration) {
		switch(announcment) {
		case Announcments.BRING_HONOR:
			StartCoroutine(tempdisplay(0, duration));
			break;
		case Announcments.DOUBLE_KILL:
			StartCoroutine(tempdisplay(1, duration));
			break;
		case Announcments.FIRST_BLOOD:
			StartCoroutine(tempdisplay(2, duration));
			break;
		case Announcments.GO:
			StartCoroutine(tempdisplay(3, duration));
			break;
		case Announcments.GOJIRA_KILL:
			StartCoroutine(tempdisplay(4, duration));
			break;
		case Announcments.LOTS_OF_KILL_LITTLE_TIME:
			StartCoroutine(tempdisplay(5, duration));
			break;
		case Announcments.MANY_KILL:
			StartCoroutine(tempdisplay(6, duration));
			break;
		case Announcments.READY:
			StartCoroutine(tempdisplay(7, duration));
			break;
		case Announcments.SET:
			StartCoroutine(tempdisplay(8, duration));
			break;
		case Announcments.SOON:
			StartCoroutine(tempdisplay(9, duration));
			break;
		}
	}
}
