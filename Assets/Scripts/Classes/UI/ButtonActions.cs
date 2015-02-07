using UnityEngine;
using System.Collections;

public class ButtonActions {

	public static System.Action<Menu> back { get { return _back; } }
	private static void _back(Menu m) {
		m.goBack ();
	}

	public static System.Action<Menu> toMainState { get { return _toMainState; } }
	private static void _toMainState(Menu m) {
		m.setCurrent (MenuStates.MAIN);
	}

	public static System.Action<Menu> toOptionsState { get { return _toOptionsState; } }
	private static void _toOptionsState(Menu m) {
		m.setCurrent (MenuStates.OPTIONS);
	}

	public static System.Action<Menu> exit { get { return _exit; } }
	private static void _exit(Menu m) {
		Application.Quit ();
	}

	public static System.Action<Menu> doNothing { get { return _doNothing; } }
	public static void _doNothing(Menu m) {

	}
}
