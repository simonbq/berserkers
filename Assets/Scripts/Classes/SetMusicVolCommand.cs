using UnityEngine;
using System.Collections;

public class SetMusicVolCommand : Command {

	public override void invoke(int arg) {
		AudioListener.volume = arg
	}
}
