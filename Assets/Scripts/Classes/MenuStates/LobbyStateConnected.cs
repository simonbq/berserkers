using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * TODO:
 * - ToggleReadyButton
 * - Player info field(s)
 * - Start Game (for host) -- Replace ToggleReadyButton?
 */
public class LobbyStateConnected : MenuState {
	ActionData data;
	List<Button> buttons = new List<Button>();
	Button host;
	Button ready;
	public LobbyStateConnected() {
		data = new ActionData ();
		data.gameName = "null";
		Button b;

		b = new Button (500, 300, Button.BUTTON_NORM);
		b.setText ("Disconnect");
		b.setOnClick (ButtonActions.disconnect);
		buttons.Add (b);

		ready = new Button (300, 300, Button.BUTTON_NORM);
		ready.setText ("Ready");
		ready.setOnClick (ButtonActions.ready);

		host = new Button (300, 300, Button.BUTTON_NORM);
		host.setText ("Start");
		host.setOnClick (ButtonActions.start);
	}

	public override void update(Menu m) {
		data.menu = m;

		foreach(Button b in buttons) {
			b.update(data);
		}

		if(Network.isServer) {
			host.update(data);
		} else {
			ready.update(data);
		}

		if(!Connections.GetInstance().isConnected) {
			m.goBack();
		}
	}

	public override void render() {

	}
}
