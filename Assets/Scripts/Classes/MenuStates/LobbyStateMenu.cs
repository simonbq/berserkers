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
	private Rect connArea = new Rect(960-256, 256, 512, 256);
	public LobbyStateMenu() {
		data = new ActionData ();

		Button b;

		b = new Button (960 + 256 , 256 - 32, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.connect);
		b.setText ("Connect");
		buttons.Add (b);

		b = new Button (960 - 128 - 128, 400, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.host);
		b.setText ("Host");
		buttons.Add (b);

		b = new Button (960 + 128 - 128, 400, Button.BUTTON_NORM);
		b.setOnClick (ButtonActions.back);
		b.setText ("Back");
		buttons.Add (b);
	}

	public override void update(Menu m) {
		data.menu = m;
		GUILayout.BeginArea (connArea);
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Nickname", GUILayout.Width(200));
		Connections.GetInstance ().localNickname = GUILayout.TextField (Connections.GetInstance ().localNickname, GUILayout.Width (300));
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label("IP", GUILayout.Width(200));
		ipNumber = GUILayout.TextField (ipNumber, GUILayout.Width (300));
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
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
