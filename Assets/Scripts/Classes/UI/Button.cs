using UnityEngine;
using System.Collections;

public class Button {
	public static readonly Vector2 BUTTON_NORM = new Vector2 (192, 128);
	public static readonly Vector2 BUTTON_SMALL = new Vector2 (96, 64);
	private Rect area;
	private Vector2 scaledCenter = new Vector2 ();
	private GUIContent content;
	private System.Action<ActionData> onClick;
	private float angle = 0.0f;
	public Button(int x, int y, Vector2 size) : this(new Rect(x, y, size.x, size.y)) {

	}

	public Button(int x, int y, int width, int height) : this(new Rect (x, y, width, height)) {
	}

	public Button(Rect area) {
		this.area = area;
		content = new GUIContent ();
	}

	public void setText(string text) {
		content.text = text;
	}

	public void setOnClick(System.Action<ActionData> action) {
		onClick = action;
	}

	public void calculate(ActionData data) {
		scaledCenter.x = (area.center.x * data.menu.screen_scale.x);
		scaledCenter.y = (area.center.y * data.menu.screen_scale.y);
		angle = Mathf.Cos (Time.time) * 15.0f;
	}

	// Renders and returns true if pressed
	public bool update(ActionData data) {
		
		GUIUtility.RotateAroundPivot (angle, scaledCenter);
		bool b = GUI.Button (area, content);
		GUIUtility.RotateAroundPivot (-angle, scaledCenter);
		if (b) {
			SoundStore.instance.Play(SoundStore.instance.buttonPress);

			if(onClick != null) {
				onClick.Invoke(data);
			}
			return true;
		} else {
			return false;
		}
	}
}
