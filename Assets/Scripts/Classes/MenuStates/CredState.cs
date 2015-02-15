using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CredState : MenuState {
	private string text = "Rasmus Hofreus - Design\nSebastian Jarkeborn - Programming\nErik Thomasson Forsberg - Programming\nNiklas Lindblad - Programming\nSimon Bergqvist - Programming\nViktor Engstrom - Music\nVile Frans Kaathe - Sound, Voice Acting\nViktor Zryd - Sound, Voice Acting\nNiklas Bergwall - Animation, Voice Acting\nGustav Rosberg - Animation, Modeling\nMattias Lindblad - Modeling\nEvelina Waara - Animation, 2D Art";
	private List<Button> buttons = new List<Button>();
	private ActionData data;

	public CredState() {
		Button b;
		b = new Button (960 - 128, 500, Button.BUTTON_NORM);
		b.setText ("Back");
		b.setOnClick (ButtonActions.back);
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
		GUI.Label(new Rect(Screen.width / 4,100,Screen.width,Screen.height),text);
		foreach(Button b in buttons) {
			b.update(data);
		}
	}
}
