using UnityEngine;
using System.Collections;

public struct ActionData {
	public MenuBase menu { get; set; }
	public string tarIP { get; set; }
	public string gameName { get; set; }
}

public class ButtonActions {

	public static System.Action<ActionData> back { get { return _back; } }
	private static void _back(ActionData data) {
		data.menu.goBack ();
	}

	public static System.Action<ActionData> toMainState { get { return _toMainState; } }
	private static void _toMainState(ActionData data) {
		data.menu.setCurrent (MenuStates.MAIN);
	}

	public static System.Action<ActionData> toOptionsState { get { return _toOptionsState; } }
	private static void _toOptionsState(ActionData data) {
		data.menu.setCurrent (MenuStates.OPTIONS);
	}

	public static System.Action<ActionData> toLobbyStateMenu { get { return _toLobbyStateMenu; } }
	private static void _toLobbyStateMenu (ActionData data) {
		data.menu.setCurrent (MenuStates.LOBBY_BROWSER);
	}

	public static System.Action<ActionData> exit { get { return _exit; } }
	private static void _exit(ActionData data) {
		Application.Quit ();
	}

	public static System.Action<ActionData> doNothing { get { return _doNothing; } }
	public static void _doNothing(ActionData data) {

	}

	public static System.Action<ActionData> connect { get { return _connect; } }
	private static void _connect(ActionData data) {
		Network.Connect (data.tarIP, 61337);
	}


    public static System.Action<ActionData> toDirectconnect { get { return _toDirectconnect; } }
    private static void _toDirectconnect(ActionData data)
    {
        data.menu.setCurrent(MenuStates.LOBBY_MENU);
    }

	public static System.Action<ActionData> disconnect { get { return _disconnect; } }
	private static void _disconnect(ActionData data) {
		Network.Disconnect ();
		if(data.menu != null)
			data.menu.goBack ();
	}

	public static System.Action<ActionData> host { get { return _host; } }
	private static void _host(ActionData data) {
		Connections.GetInstance().HostLobby(8);
	}

	public static System.Action<ActionData> refresh { get { return _refresh; } }
	private static void _refresh(ActionData data) {

	}

	public static System.Action<ActionData> ready { get { return _ready; } }
	private static void _ready(ActionData data) {
		Connections.GetInstance ().ToggleReady ();
	}

	public static System.Action<ActionData> start { get { return _start; } }
	private static void _start(ActionData data) {
		if(Connections.GetInstance().playersReady)
			Connections.GetInstance ().StartGame ("Level");
	}

	public static System.Action<ActionData> toggleFullscreen { get { return _toggleFullscreen; } }
	private static void _toggleFullscreen(ActionData data) {
		Screen.fullScreen = !Screen.fullScreen;
	}

	public static System.Action<ActionData> toggleMovingButtons { get { return _toggleMovingButtons; } }
	private static void _toggleMovingButtons(ActionData data) {
		Options.movingButtons = !Options.movingButtons;
		Prefs.SetButton (Options.movingButtons);
		Prefs.Save ();
	}

	public static System.Action<ActionData> toggleMovingMenu { get { return _toggleMovingMenu; } }
	private static void _toggleMovingMenu(ActionData data) {
		Options.movingMenu = !Options.movingMenu;
		Prefs.SetBg (Options.movingMenu);
		Prefs.Save ();
	}

	public static System.Action<ActionData> toggleDynamicHUD { get { return _toggleDynamicHUD; } }
	private static void _toggleDynamicHUD(ActionData data) {
		Options.dynamicHud = !Options.dynamicHud;
		Prefs.SetHud (Options.dynamicHud);
		Prefs.Save ();
	}

    public static System.Action<ActionData> refreshHostlist { get { return _refreshHostlist; } }
    private static void _refreshHostlist(ActionData data)
    {
        Connections.GetInstance().RefreshHostList();
    }
}
