using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * TODO
 * - Host button
 * - Set nickname label
 * - Refresh servers button
 * - Set IP Label
 * - Connect Button
 */
public class LobbyStateMenu : MenuState {

	List<Button> buttons = new List<Button>();
	ActionData data;
	string ipNumber = "127.0.0.1";
	public LobbyStateMenu() {
		data = new ActionData ();

		Button b;

		b = new Button (50, 50, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.connect);
		b.setText ("Connect");
		buttons.Add (b);

		b = new Button (200, 50, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.host);
		b.setText ("Host");
		buttons.Add (b);

		b = new Button (200, 200, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.back);
		b.setText ("Back");
		buttons.Add (b);
	}

	public override void update(Menu m) {
		data.menu = m;

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Nickname");
		Connections.GetInstance ().localNickname = GUILayout.TextField (Connections.GetInstance ().localNickname, GUILayout.Width (300));
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
		GUILayout.Label("IP");
		ipNumber = GUILayout.TextField (ipNumber, GUILayout.Width (300));
		GUILayout.EndHorizontal();

		data.tarIP = ipNumber;
		foreach (Button b in buttons) {
			b.update(data);
		}

		if(Connections.GetInstance().isConnected) {
			m.setCurrent(MenuStates.LOBBY_CONNECTED);
		}
	}

	public override void render() {

	}
}
