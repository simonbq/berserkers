using UnityEngine;
using System.Collections;

public class CredState : MenuState {
	public override void update(MenuBase m) {
	}

	public override void render() {
		GUI.Label(new Rect(Screen.width / 4,Screen.height / 4,(Screen.width / 4) * 3,(Screen.height / 4) * 3),"text\nhej");
	}
}
