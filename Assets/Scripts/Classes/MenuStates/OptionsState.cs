﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsState : MenuState {

	List<Button> buttons = new List<Button>();
	Slider volumeSlider;

	public OptionsState() {
		Button b;
		b = new Button (100, 100, 200, 200);
		b.setText ("Back");
		b.setOnClick (ButtonActions.back);
		buttons.Add (b);
		volumeSlider = new Slider (100, 400, 100, 50, 0, 1);
	}

	public override void update(Menu m) {
		foreach(Button b in buttons) {
			b.update(m);
		}
		AudioListener.volume = volumeSlider.update (AudioListener.volume);
	}
	
	public override void render() {

	}
}
