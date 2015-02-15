using UnityEngine;
using System.Collections;

public class HowState : MenuState {
	public override void update(MenuBase m) {
	}
	
	public override void render() {
		GUI.Label(new Rect(Screen.width / 2 - 512,200,Screen.width,Screen.height),"text\nhej");
	}
}
