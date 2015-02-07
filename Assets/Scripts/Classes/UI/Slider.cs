using UnityEngine;
using System.Collections;

public class Slider {

	Rect rect;
	Rect textRect;
	float start;
	float end;

	public Slider(int x, int y, int width, int height, float start, float end) : this(new Rect(x, y, width, height), start, end){
	}

	public Slider(Rect rect, float start, float end) {
		this.rect = rect;
		textRect = new Rect (rect);
		this.start = start;
		this.end = end;
	}

	public float update(float current) {
		
		return GUI.HorizontalSlider (rect, current, start, end);
	}
}
