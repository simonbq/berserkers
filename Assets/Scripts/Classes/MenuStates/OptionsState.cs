using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsState : MenuState {

	private List<Button> buttons = new List<Button>();
	private Slider volumeSlider;
	private Slider graphicsSlider;
	private ActionData data;

	public OptionsState() {
		Button b;
		b = new Button (100, 100, Button.BUTTON_NORM);
		b.setText ("Back");
		b.setOnClick (ButtonActions.back);
		buttons.Add (b);
		volumeSlider = new Slider (100, 400, 100, 50, 0, 1, "Volume");
		graphicsSlider = new Slider(100, 500, 100, 50, 0, 1, "Graphics");
		data = new ActionData ();
	}

	public override void update(Menu m) {
		data.menu = m;
		foreach(Button b in buttons) {
			b.update(data);
		}
		AudioListener.volume = volumeSlider.update (AudioListener.volume);
		float v = (float)QualitySettings.GetQualityLevel () / (QualitySettings.names.Length-1);
		QualitySettings.SetQualityLevel ((int)(graphicsSlider.update (v) * (int)QualitySettings.names.Length));
	}
	
	public override void render() {

	}
}
