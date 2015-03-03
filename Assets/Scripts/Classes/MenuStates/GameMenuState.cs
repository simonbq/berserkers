using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuState : MenuState {
	ActionData data;
	private List<Button> buttons = new List<Button>();
	public GameMenuState() {
		data = new ActionData ();
		Button b;

		b = new Button ((int)Menu.SCREEN_SIZE.x / 2 - 128 - 32, 300, (int)Button.BUTTON_NORM.x+64, 128, 30.0f, 5.0f);
		b.setText("Disconnect");
		b.setOnClick (ButtonActions.disconnect);
		buttons.Add (b);

		b = new Button ((int)Menu.SCREEN_SIZE.x / 2 - 128, 200, (int)Button.BUTTON_NORM.x, 64, 30.0f, 5.0f);
		b.setText ("Dyn. HUD");
		b.setOnClick (ButtonActions.toggleDynamicHUD);
		buttons.Add (b);

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

	}
}
