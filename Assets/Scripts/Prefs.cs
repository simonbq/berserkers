using UnityEngine;
using System.Collections;

public class Prefs : MonoBehaviour {

	public static float GetVolume() {
		return PlayerPrefs.GetFloat ("volume", 0.5f);
	}

	public static string GetName() {
		return PlayerPrefs.GetString ("name", "Unnamed");
	}

	public static void SetVolume(float vol) {
		PlayerPrefs.SetFloat ("volume", vol);
	}

	public static void SetName(string name) {
		PlayerPrefs.SetString ("name", name);
	}

	public static void Save() {
		PlayerPrefs.Save ();
	}

	public static void SetButton(bool b) {
		int i = BoolToInt (b);
		PlayerPrefs.SetInt ("button", i);
	}

	public static void SetBg(bool b) {
		int i = BoolToInt (b);
		PlayerPrefs.SetInt ("bg", i);
	}

	public static void SetHud(bool b) {
		int i = BoolToInt (b);
		PlayerPrefs.SetInt ("hud", i);
	}

	public static bool GetButton() {
		bool b = IntToBool (PlayerPrefs.GetInt ("button", 1));
		return b;
	}

	public static bool GetBg() {
		bool b = IntToBool(PlayerPrefs.GetInt("bg", 1));
		return b;
	}

	public static bool GetHud() {
		bool b = IntToBool(PlayerPrefs.GetInt("hud", 1));
		return b;
	}

	private static bool IntToBool(int i) {
		if (i == 0) return false;
		else return true;
	}

	private static int BoolToInt(bool b) {
		if (b) return 1;
		else return 0;
	}
}
