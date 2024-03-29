﻿using UnityEngine;
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
public class LobbyStateBrowser : MenuState
{

    List<Button> buttons = new List<Button>();
    ActionData data;
    string ipNumber = "127.0.0.1";
    private Rect connArea = new Rect(960 - 256, 64, 512 + 64, 320);
    private Vector3 lobbyScroll = new Vector2();

	private Rect errRect = new Rect(64, 128, 512, 256);
	private float errMsgEnd = -1.0f;
	private string errMsg = "";
    public static LobbyStateBrowser instance;
    public LobbyStateBrowser()
    {
        instance = this;
        data = new ActionData();

        Button b;

        b = new Button(960 + 320, 128, Button.BUTTON_NORM);
        b.setOnClick(ButtonActions.refreshHostlist);
        b.setText("Refresh");
        buttons.Add(b);

        b = new Button(0 + 960 - 128, 500, Button.BUTTON_NORM);
        b.setOnClick(ButtonActions.toDirectconnect);
        b.setText("IP connect");
        buttons.Add(b);

        b = new Button(-256 + 960 - 128, 500, Button.BUTTON_NORM);
        b.setOnClick(ButtonActions.host);
        b.setText("Host");
        buttons.Add(b);

        b = new Button(256 + 960 - 128, 500, Button.BUTTON_NORM);
        b.setOnClick(ButtonActions.back);
        b.setText("Back");
        buttons.Add(b);
    }

    public override void update(MenuBase m)
    {
        data.menu = m;
        data.tarIP = ipNumber;

        foreach (Button b in buttons)
        {
            b.calculate(data);
        }
        if (Connections.GetInstance().isConnected)
        {
            m.setCurrent(MenuStates.LOBBY_CONNECTED);
        }
    }

    public override void render()
    {
        GUILayout.BeginArea(connArea);
        GUILayout.Label("Open lobbies");
        lobbyScroll = GUILayout.BeginScrollView(lobbyScroll);
		
		var serverList = Connections.GetInstance ().serverList;

		if(serverList != null)
		{
	        foreach (HostData lobby in serverList)
	        {
	            if (GUILayout.Button(lobby.gameName + " (" + lobby.connectedPlayers + " / " + (lobby.playerLimit - 1) + ")"))
	            {
	                Network.Connect(lobby);
                    displayError("Connecting...");
	            }
	        }
		}

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nickname", GUILayout.Width(200));
        Connections.GetInstance().localNickname = GUILayout.TextField(Connections.GetInstance().localNickname, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        foreach (Button b in buttons)
        {
            b.update(data);
        }

		if(errMsgEnd > Time.time) {
			GUI.Box(errRect, errMsg, GUI.skin.customStyles[1]);
		}
    }

	public void displayError(string msg) {
		errMsg = msg;
		errMsgEnd = Time.time + 7.0f;
	}

    public void removeError()
    {
        errMsgEnd = Time.time;
    }

}