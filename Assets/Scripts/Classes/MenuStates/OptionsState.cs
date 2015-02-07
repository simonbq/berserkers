using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsState : MenuState {

	List<Button> buttons = new List<Button>();
	Slider volumeSlider;
	Slider graphicsSlider;

	public OptionsState() {
		Button b;
		b = new Button (100, 100, 200, 200);
		b.setText ("Back");
		b.setOnClick (ButtonActions.back);
		buttons.Add (b);
		volumeSlider = new Slider (100, 400, 100, 50, 0, 1, "Volume");
		graphicsSlider = new Slider(100, 500, 100, 50, 0, 1, "Graphics");
	}

	public override void update(Menu m) {
		foreach(Button b in buttons) {
			b.update(m);
		}
		AudioListener.volume = volumeSlider.update (AudioListener.volume);
		float v = (float)QualitySettings.GetQualityLevel () / (QualitySettings.names.Length-1);
		QualitySettings.SetQualityLevel ((int)(graphicsSlider.update (v) * (int)QualitySettings.names.Length));
	}
	
	public override void render() {

	}
}
