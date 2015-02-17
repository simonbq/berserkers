using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Announcments {
	BRING_HONOR, DOUBLE_KILL, FIRST_BLOOD, GO, GOJIRA_KILL,
	LOTS_OF_KILL_LITTLE_TIME, MANY_KILL, READY, SET, SOON
}

public class Ambulance : Starlike {
	public Rect rect;
	private Rect texRect;
	private int stride;
	private Texture tex;
	private bool dead = false;

	private static readonly float timePerFrame = 0.2f;
	private float loopTime;
	private float slides;
	private float startTime;
	public Ambulance(Rect rect, Texture tex, int slides, int size) {
		this.rect = rect;
		this.tex = tex;
		this.stride = size;
		this.texRect = new Rect (0, 0, 1.0f / (float)slides, 1);
		this.loopTime = slides * timePerFrame;
		this.slides = slides;
		this.startTime = Time.time;
	}
	
	public override void gui() {
		texRect.x = ((int)(((Time.time - startTime) / timePerFrame) % loopTime)) / slides;
		//Debug.Log (texRect.x);
		GUI.DrawTextureWithTexCoords (rect, tex, texRect);
	}
	
	public void die() {
		dead = true;
		if(previous != null) {
			previous.next = next;
		}
		if(next != null) {
			next.previous = previous;
		}
	}
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
	public float quote_maxSpeed_endquote = 9000.0f;

	public Rect gurka_rect = new Rect (0, 0, 525, 292);
	public Texture gurka_tex;

	public Texture spinner_TR_tex;
	public Rect spinner_TR_rect = new Rect (0, 0, 512, 512);
	public float spinner_TR_speed = 30.0f;
	public Texture spinner_BL_tex;
	public Rect spinner_BL_rect = new Rect (0, 0, 512, 512);
	public float spinner_BL_speed = 30.0f;

	public Rect announcerRect = new Rect (800, 300, 256, 512);


	public Texture nosmoke_tex;
	public Rect nosmoke_rect = new Rect(0, 0, 0, 0);

	public Texture[] ambulances;
	public int ambulancesPerSheet = 24;
	public int ambulanceSize = 128;
	public float ambulanceDuration = 1.0f;
	public AnimationCurve ambulanceHopCurve;
	public AnimationCurve ambulanceSpeedCurve;
	private StarAnchor ambulance_list = new StarAnchor();

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

	private float next = 0.0f;
	// Update is called once per frame
	void Update () {
		if(Time.time > next) {
			next = Time.time + 5.0f;
			StartCoroutine(convoy(Random.Range(1, 5)));
		}
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

	private void RotateAroundDraw(float rot, Rect rect, Texture tex) {
		GUIUtility.RotateAroundPivot (rot, new Vector2(rect.center.x * scale.x, rect.center.y * scale.y));
		GUI.DrawTexture (rect, tex);
		GUIUtility.RotateAroundPivot (-rot, new Vector2(rect.center.x * scale.x, rect.center.y * scale.y));
	}
	
	
	void OnGUI() {
		GUI.matrix = matrix;
		if(skin != null) {
			GUI.skin = skin;
		}



		if(Options.dynamicHud) {
			float rotate = Time.time;
			RotateAroundDraw(rotate * spinner_TR_speed, spinner_TR_rect, spinner_TR_tex);
			RotateAroundDraw(rotate * spinner_BL_speed, spinner_BL_rect, spinner_BL_tex);
		} else {
			GUI.DrawTexture (spinner_TR_rect, spinner_TR_tex);
			GUI.DrawTexture (spinner_BL_rect, spinner_BL_tex);
		}

		ambulance_list.gui ();

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
			GUIUtility.ScaleAroundPivot(-Vector2.one, speedometer_rect.center);
			if(Event.current.rawType == EventType.repaint)
				Graphics.DrawTexture (speedometer_rect, speedometer_mat.mainTexture, speedometer_mat);
			GUIUtility.ScaleAroundPivot(Vector2.one, speedometer_rect.center);
			GUI.Label (speedometer_textrect, "N/A", GUI.skin.customStyles [1]); //
		}

		GUI.DrawTexture (gurka_rect, gurka_tex);

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

		GUI.DrawTexture (BACKGROUND_AREA, statics);

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

	private IEnumerator convoy(int amount, float interval = 0.5f) {
		while(amount > 0) {
			StartCoroutine(ambulance());
			amount--;
			yield return new WaitForSeconds(interval);
		}
	}

	private IEnumerator ambulance() {
		if(Options.dynamicHud) {
			Ambulance s = new Ambulance (new Rect (SCREEN_SIZE.x + 100, SCREEN_SIZE.y - ambulanceSize, ambulanceSize, ambulanceSize), ambulances[Random.Range(0, ambulances.Length)], ambulancesPerSheet, ambulanceSize);
			ambulance_list.Add(s);
			float delta = 0.0f;
			while(delta < ambulanceDuration) {
				delta += Time.deltaTime;
				s.rect.x = (1 - ambulanceSpeedCurve.Evaluate( delta/ambulanceDuration)) * (SCREEN_SIZE.x + ambulanceSize) - ambulanceSize;
				s.rect.y = ambulanceHopCurve.Evaluate(delta/ambulanceDuration) * ambulanceSize + SCREEN_SIZE.y - ambulanceSize;
				yield return null;
			}
			s.die ();
		}
	}

	public void spawnAmbulance() {
		StartCoroutine (ambulance());
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
