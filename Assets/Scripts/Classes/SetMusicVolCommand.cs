using UnityEngine;
using System.Collections;

public class SetMusicVolCommand : Command {

	public override void invoke(float arg) {
		AudioListener.volume = arg;
	}
}
