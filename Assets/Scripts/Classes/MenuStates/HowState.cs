using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HowState : MenuState {
	private string text = "BERZERKURUUU\n\nSteer your berserker right and left\nAvoid walls and rocks\nPick up flaming spheres to increase your speed\nDefeat your opponents by being faster when colliding\n\nDefault controls:\nPlayer 1: A-D\nPlayer 2: Left-Right\nPlayer 3: K-L\nPlayer 4: G-H";
	private List<Button> buttons = new List<Button>();
	private ActionData data;

	public HowState() {
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
