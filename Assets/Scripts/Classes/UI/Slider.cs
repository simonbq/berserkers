using UnityEngine;
using System.Collections;

public class Slider {

	Rect rect;
	Rect textRect;
	float start;
	float end;
	string title;

	public Slider(int x, int y, int width, int height, float start, float end, string title) : this(new Rect(x, y, width, height), start, end, title){
	}

	public Slider(Rect rect, float start, float end, string title) {
		this.rect = rect;
		textRect = new Rect (rect);
		textRect.y -= 20;
		textRect.height = 20;
		this.start = start;
		this.end = end;
		this.title = title;
	}

	public float update(float current) {
		GUI.Label (textRect, title);
		return GUI.HorizontalSlider (rect, current, start, end);
	}
}
