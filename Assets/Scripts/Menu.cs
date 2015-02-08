using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment;

public class Star {
	public Rect rect;
	private Texture tex;
	public Star(Rect rect, Texture tex) {
		this.rect = rect;
		this.tex = tex;
	}

	public void gui() {
		GUI.DrawTexture (rect, tex);
	}
}

public class Menu : MonoBehaviour {
	public static readonly Vector2 SCREEN_SIZE = new Vector2 (1920, 1080);
	private Vector2 screenScale;
	public Texture background;
	public Texture spinThing;
	public Texture TitleText;
	public Texture[] stars;
	private MenuState current;
	private Stack<MenuState> history = new Stack<MenuState>();
	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();

	private static readonly Rect BACKGROUND_AREA = new Rect (0, 0, 1920, 1080);
	private Rect spinThingArea = new Rect(0, 0, 4000, 4000);
	private Rect titleArea = new Rect (0, 0, 1596, 453);
	public AnimationCurve titleAwesomenessCurve;
	// Use this for initialization
	void Awake () {
		spinThingArea.center = SCREEN_SIZE / 2;
		spinThingArea.y += 800;
		spinThingArea.x += 300;

		titleArea.center = SCREEN_SIZE / 2;
		titleArea.y += 300;

		Debug.Log (titleArea.center);
		current = MenuStates.MAIN;

	}
	
	// Update is called once per frame
	void Update () {
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		screenScale = new Vector2 (width, height);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));
	}

	void OnGUI() {
		GUI.matrix = matrix;
		if(skin != null) {
			GUI.skin = skin;
		}
		float rotate = Time.time * 57;
		float scalify = titleAwesomenessCurve.Evaluate ((Time.time/2.0f) % 1);
		GUI.DrawTexture (BACKGROUND_AREA, background);
		GUIUtility.RotateAroundPivot (rotate, new Vector2(spinThingArea.center.x * scale.x, spinThingArea.center.y * scale.y));
		GUI.DrawTexture (spinThingArea, spinThing);
		GUIUtility.RotateAroundPivot (-rotate, new Vector2(spinThingArea.center.x * scale.x, spinThingArea.center.y * scale.y));
		current.update (this);
		GUIUtility.ScaleAroundPivot (Vector2.one * scalify, titleArea.center);//new Vector2(titleArea.center.x * scale.x, titleArea.center.y * scale.y));
		GUI.DrawTexture (titleArea, TitleText);
		GUIUtility.ScaleAroundPivot (-Vector2.one * scalify, titleArea.center);
		//GUIUtility.ScaleAroundPivot (-(Vector2.one * scalify), new Vector2(titleArea.center.x * scale.x, titleArea.center.y * scale.y));

	}

	private IEnumerator star() {
		/*Star s = new Star (new Rect (-100, Random.Range (100, 900), 64, 64));
		while(s.rect.x < SCREEN_SIZE.x + 100) {

			yield return null;
		}*/
		yield return null;
	}

	public void setCurrent(MenuState state) {
		history.Push (current);
		current = state;
	}

	public void goBack() {
		current = history.Pop ();
	}
}
