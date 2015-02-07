using UnityEngine;
using System.Collections;

public class Button {
	private Rect area;
	private GUIContent content;
	private System.Action onClick;
	public Button(int x, int y, int width, int height) : this(new Rect (x, y, width, height)) {
	}

	public Button(Rect area) {
		this.area = area;
	}

	public void setText(string text) {
		content.text = text;
	}

	public void setOnClick(System.Action action) {
		onClick = action;
	}

	// Renders and returns true if pressed
	public bool update() {
		if (GUI.Button (area, content)) {
			if(onClick != null) {
				onClick.Invoke();
			}
			return true;
		} else {
			return false;
		}
	}
}
