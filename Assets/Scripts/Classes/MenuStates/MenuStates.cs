using UnityEngine;
using System.Collections;

public class MenuStates {
	public static readonly MenuState OPTIONS = new OptionsState();
	public static readonly MenuState MAIN = new MainState();
	public static readonly MenuState LOBBY_MENU = new LobbyStateMenu();
    public static readonly MenuState LOBBY_BROWSER = new LobbyStateBrowser();
	public static readonly MenuState LOBBY_CONNECTED = new LobbyStateConnected();
	public static readonly MenuState CRED = new CredState();
	public static readonly MenuState HOW = new HowState();
}
