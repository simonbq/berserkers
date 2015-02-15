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
		b = new Button (960 - 128, 400, Button.BUTTON_NORM);
		b.setText ("OK");
		b.setOnClick (ButtonActions.back);
		buttons.Add (b);

		b = new Button (960 - 128, 300, (int)Button.BUTTON_NORM.x, 64);
		b.setText ("Fullscreen");
		b.setOnClick (ButtonActions.toggleFullscreen);
		buttons.Add (b);

		b = new Button (960 - 128 + 256, 300, (int)Button.BUTTON_NORM.x, 64);
		b.setText ("Dyn. Buttons");
		b.setOnClick (ButtonActions.toggleMovingButtons);
		buttons.Add (b);

		b = new Button (960 - 128 + 256, 200, (int)Button.BUTTON_NORM.x, 64);
		b.setText ("Dyn. Menu");
		b.setOnClick (ButtonActions.toggleMovingMenu);
		buttons.Add (b);

		//Ingen feedback så tog bort, använd ingame istället
		/*b = new Button (960 - 128 + 256, 100, (int)Button.BUTTON_NORM.x, 64);
		b.setText ("Dyn. HUD");
		b.setOnClick (ButtonActions.toggleDynamicHUD);
		buttons.Add (b);*/

		volumeSlider = new Slider (960 - 160, 100, 256, 128, 0, 1, "Volume");
		graphicsSlider = new Slider(960 - 160, 200, 256, 128, 0, 1, "Graphics");
		data = new ActionData ();
	}

	public override void update(MenuBase m) {
		data.menu = m;
		foreach(Button b in buttons) {
			b.calculate(data);
		}

	}
	
	public override void render() {
		foreach(Button b in buttons) {
			b.update(data);
		}
		AudioListener.volume = volumeSlider.update (AudioListener.volume);
		float v = (float)QualitySettings.GetQualityLevel () / (QualitySettings.names.Length-1);
		QualitySettings.SetQualityLevel ((int)(graphicsSlider.update (v) * (int)QualitySettings.names.Length));
	}
}
