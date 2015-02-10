using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuState : MenuState {
	ActionData data;
	private List<Button> buttons = new List<Button>();
	public GameMenuState() {
		data = new ActionData ();
		Button b;

		b = new Button ((int)Menu.SCREEN_SIZE.x / 2, 300, 256, 128, 0.0f, 0.0f);
		b.setText("Disconnect");
		b.setOnClick (ButtonActions.disconnect);
	}

	public override void update(Menu m) {
		data.menu = m;
	}

	public override void render() {

	}
}
