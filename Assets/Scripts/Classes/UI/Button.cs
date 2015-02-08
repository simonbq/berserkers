using UnityEngine;
using System.Collections;

public class Button {
	public static readonly Vector2 BUTTON_NORM = new Vector2 (192, 128);
	private Rect area;
	private GUIContent content;
	private System.Action<ActionData> onClick;

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

	// Renders and returns true if pressed
	public bool update(ActionData data) {
		if (GUI.Button (area, content)) {
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
