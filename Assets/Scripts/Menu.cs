using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment;

public abstract class Starlike {
	public Starlike next = null;
	public Starlike previous = null;
	public abstract void gui();
}

public class Star : Starlike {
	public Rect rect;


	private Texture tex;
	private bool dead = false;
	public Star(Rect rect, Texture tex) {
		this.rect = rect;
		this.tex = tex;
	}

	public override void gui() {
		GUI.DrawTexture (rect, tex);
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

public class StarAnchor : Starlike {

	public void Add(Star star) {
		if(next == null) {
			next = star;
			next.previous = this;
		} else {
			Starlike s = next;
			while(s.next != null) {
				s = s.next;
			}
			s.next = star;
			star.previous = s;
		}
	}

	public override void gui() {
		Starlike s = next;
		while(s != null) {
			s.gui();
			s = s.next;
		}
	}
}

public class Menu : MenuBase {
	public Texture background;
	public Texture spinThing;
	public Texture TitleText;
	public Texture[] stars;
	public float starSpeed = 5000.0f;

	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();
	private static readonly Rect BACKGROUND_AREA = new Rect (0, 0, 1920, 1080);
	private Rect spinThingArea = new Rect(0, 0, 5000, 5000);
	private Rect titleArea = new Rect (0, 0, 1596, 453);
	public AnimationCurve titleAwesomenessCurve;

	public float avgStarSpawnTime = 0.7f;
	public int starsPerSpawn = 2;
	private float nextStar = 0.0f;

	private StarAnchor starsAnchor = new StarAnchor();
	// Use this for initialization
	void Awake () {
		spinThingArea.center = SCREEN_SIZE / 2;
		spinThingArea.y += 800;
		spinThingArea.x += 300;

		titleArea.center = SCREEN_SIZE / 2;
		titleArea.y += 300;
		setCurrent (MenuStates.MAIN);
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		screenScale = new Vector2 (width, height);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > nextStar) {
			nextStar = Time.time + avgStarSpawnTime;
			for(int i = 0; i < starsPerSpawn; i++) {
				StartCoroutine(star());
			}

		}
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		screenScale = new Vector2 (width, height);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));
		current.update (this);
	}

	void OnGUI() {
		GUI.matrix = matrix;
		if(skin != null) {
			GUI.skin = skin;
		}
		float rotate = Time.time * 57;
		float scalify = titleAwesomenessCurve.Evaluate ((Time.time/2.0f) % 1);
		GUI.DrawTexture (BACKGROUND_AREA, background);
		GUI.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
		starsAnchor.gui ();
		GUI.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		if(Options.movingMenu) {
			GUIUtility.RotateAroundPivot (rotate, new Vector2(spinThingArea.center.x * scale.x, spinThingArea.center.y * scale.y));
		}
		GUI.DrawTexture (spinThingArea, spinThing);
		if(Options.movingMenu) {
			GUIUtility.RotateAroundPivot (-rotate, new Vector2(spinThingArea.center.x * scale.x, spinThingArea.center.y * scale.y));
		}
		current.render ();

		if(Options.movingMenu) {
			GUIUtility.ScaleAroundPivot (Vector2.one * scalify, titleArea.center);//new Vector2(titleArea.center.x * scale.x, titleArea.center.y * scale.y));
		}
		GUI.DrawTexture (titleArea, TitleText);
		if(Options.movingMenu) {
			GUIUtility.ScaleAroundPivot (Vector2.one * (1.0f/scalify), titleArea.center);
		}
		//GUIUtility.ScaleAroundPivot (-(Vector2.one * scalify), new Vector2(titleArea.center.x * scale.x, titleArea.center.y * scale.y));

	}

	private IEnumerator star() {
		Star s = new Star (new Rect (Random.Range (-32, 1952), 1200, Random.Range(32, 64), Random.Range(32, 64)), stars[Random.Range(0, stars.Length)]);
		starsAnchor.Add (s);
		float v = Random.Range (0.8f, 1.0f);
		while(s.rect.y+ 100 > 0) {
			s.rect.y -= Time.deltaTime * v * starSpeed;
			yield return null;
		}
		s.die();
		yield return null;
	}
}
