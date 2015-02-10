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
	private float rotationSpeed;
	private float rotationAngle;
	public Button(int x, int y, Vector2 size, float rotationSpeed = 1.0f, float rotationAngle = 15.0f) : this(new Rect(x, y, size.x, size.y), rotationSpeed, rotationAngle) {

	}

	public Button(int x, int y, int width, int height, float rotationSpeed = 1.0f, float rotationAngle = 15.0f) : this(new Rect (x, y, width, height), rotationSpeed, rotationAngle) {
	}

	public Button(Rect area, float rotationSpeed = 1.0f, float rotationAngle = 15.0f) {
		this.area = area;
		content = new GUIContent ();
		this.rotationSpeed = rotationSpeed;
		this.rotationAngle = rotationAngle;
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
		angle = Mathf.Cos (Time.time * rotationSpeed) * rotationAngle;
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
