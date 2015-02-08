﻿using UnityEngine;
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
	Rect usersArea = new Rect(960 - 512, 32, 512, 512);
	public LobbyStateConnected() {
		data = new ActionData ();
		data.gameName = "null";
		Button b;
		
		b = new Button (960 + 64, 256 + 128, Button.BUTTON_NORM);
		b.setText ("Disconnect");
		b.setOnClick (ButtonActions.disconnect);
		buttons.Add (b);

		ready = new Button (960 + 64, 192, Button.BUTTON_NORM);
		ready.setText ("Ready");
		ready.setOnClick (ButtonActions.ready);

		host = new Button (960 + 64, 192, Button.BUTTON_NORM);
		host.setText ("Start");
		host.setOnClick (ButtonActions.start);
	}

	public override void update(Menu m) {
		data.menu = m;
		GUILayout.BeginArea (usersArea);
		foreach(PlayerInfo p in Connections.GetInstance().players.Values)
		{
			GUILayout.Box(p.name + " | ID: " + p.id + " | Ready: " + p.ready);
		}
		GUILayout.EndArea ();

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
