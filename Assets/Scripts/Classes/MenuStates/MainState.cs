using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainState : MenuState {

	List<Button> buttons = new List<Button>();
	
	public MainState() {
		Button b;
		b = new Button (100, 100, 200, 200);
		b.setText ("Options");
		b.setOnClick (ButtonActions.toOptionsState);
		buttons.Add (b);

		b = new Button (400, 400, 200, 200);
		b.setText ("Exit");
		b.setOnClick (ButtonActions.exit);
		buttons.Add (b);
	}
	
	public override void update(Menu m) {
		foreach(Button b in buttons) {
			b.update(m);
		}
	}
	
	public override void render() {
		
	}
}
