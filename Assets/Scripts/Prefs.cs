using UnityEngine;
using System.Collections;

public class Prefs : MonoBehaviour {

	public static float GetVolume() {
		return PlayerPrefs.GetFloat ("volume", 1f);
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
}
