using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsState : MenuState {

	List<Button> buttons = new List<Button>();

	public OptionsState() {
		Button b;
		b = new Button (100, 100, 200, 200);
		b.setText ("HAI");
		b.setOnClick (foo);
		buttons.Add (b);
	}

	public override void update(Menu m) {
		
	}
	
	public override void render() {

	}
	

	private static void foo() {

	}
}
