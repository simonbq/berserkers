using UnityEngine;
using System.Collections;

public class SoundStore : MonoBehaviour {
	public static SoundStore instance { get; private set; } 

	public AudioClip buttonPress;
	public AudioClip[] KillSound;
	public AudioClip[] KillShout;
	public AudioClip[] DeathShout;
	public AudioClip[] StunSound;
	public AudioClip[] StunShout;
	public AudioClip[] PowerUpPickUpSound;
	public AudioClip[] PowerUpPickUpShout;
	public AudioClip PowerUpSpawn;
	public AudioClip AnnouncerStart;
	public AudioClip AnnouncerFirstBlood;
	public AudioClip AnnouncerDoubleKill;
	public AudioClip AnnouncerMultiKill;
	public AudioClip AnnouncerThreeKills;
	public AudioClip AnnouncerFiveKills;
	public AudioClip AnnouncerSevenKills;
	public AudioClip AnnouncerNoKill;
	public AudioClip SonicBoom;
	public AudioClip WinSound;




	// Use this for initialization
	void Awake () {
		instance = this;
	}

	public void Play(AudioClip clip) {
		if(clip != null)
			audio.PlayOneShot (clip);
	}

	public void PlayRandom(AudioClip[] clip) {

		if (clip.Length > 0) {
			audio.PlayOneShot(clip[Random.Range(0, clip.Length)]);
		}
	}
}
