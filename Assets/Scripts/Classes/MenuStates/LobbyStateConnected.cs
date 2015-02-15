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
    Vector2 lobbyScroll = new Vector2();
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

	public override void update(MenuBase m) {
		data.menu = m;
		foreach(Button b in buttons) {
			b.calculate(data);
		}
		
		if(Network.isServer) {
			host.calculate(data);
		} else {
			ready.calculate(data);
		}

		if(!Connections.GetInstance().isConnected) {
			m.goBack();
		}

        if (!Connections.GetInstance().localPlayers.Exists(x => x.ready))
        {
            ready.setText("Ready");
        }

        else
        {
            ready.setText("Not ready");
        }

        if (Connections.GetInstance().playersReady)
        {
            host.setText("Start");
        }

        else
        {
            host.setText("Waiting");
        }
	}

	public override void render() {
		GUILayout.BeginArea (usersArea);
        lobbyScroll = GUILayout.BeginScrollView(lobbyScroll);
		foreach(PlayerInfo p in Connections.GetInstance().players.Values)
		{
			string text = p.name + " | ";
			text += p.ready ? "Ready" : "Not ready";
			GUIShadow.LayoutLabel(text);
		}
        GUILayout.EndScrollView();

		if (Connections.GetInstance().localPlayers.Count > 1 &&
		    GUILayout.Button("Remove local player"))
		{
			Connections.GetInstance().KickLocalPlayer();
		}
		if (GUILayout.Button("Add local player"))
        {
            Connections.GetInstance().AddLocalPlayer();
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
	}
}
