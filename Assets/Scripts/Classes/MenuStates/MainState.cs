using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainState : MenuState {

	private List<Button> buttons = new List<Button>();
	private ActionData data;
	public MainState() {
		Button b;
		b = new Button (0 + 960 - 128, 400, Button.BUTTON_NORM);
		b.setText ("Options");
		b.setOnClick (ButtonActions.toOptionsState);
		buttons.Add (b);

		b = new Button (-256 + 960 - 128, 400, Button.BUTTON_NORM);
		b.setText ("Play");
		b.setOnClick (ButtonActions.toLobbyStateMenu);
		buttons.Add (b);

		b = new Button (256 + 960 - 128, 400, Button.BUTTON_NORM);
		b.setText ("Exit");
		b.setOnClick (ButtonActions.exit);
		buttons.Add (b);

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
	}
}
