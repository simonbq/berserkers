using UnityEngine;
using System.Collections;

public class SoundStore : MonoBehaviour {
	public static SoundStore instance { get; private set; } 

	public AudioClip buttonPress;
	public AudioClip[] PositiveCheer;
	public AudioClip[] NegativeCheer; 
	public AudioClip[] KillSound;
	public AudioClip[] OverkillSound;
	public AudioClip[] KillShout;
	public AudioClip[] DeathShout;
	public AudioClip[] StunSoundWall;
	public AudioClip[] StunSoundPlayer;
	public AudioClip[] StunShout;
	public AudioClip[] KillWall;
	public AudioClip[] PowerUpPickUpSound;
	public AudioClip[] PowerUpPickUpShout;
	public AudioClip PowerUpSpawn;
	public AudioClip AnnouncerStart;//When players can start to move in the beginning of the round
	public AudioClip AnnouncerFirstBlood;
	public AudioClip AnnouncerDoubleKill;
	public AudioClip AnnouncerMultiKill;
	public AudioClip AnnouncerThreeKills;
	public AudioClip AnnouncerFiveKills;
	public AudioClip AnnouncerSevenKills;
	public AudioClip AnnouncerNoKill;
	public AudioClip SonicBoom;
	public AudioClip WinSound;

	public AudioClip SplashScreenSound;
	public AudioClip MatchStart;


	// Use this for initialization
	void Awake () {
		instance = this;
        DontDestroyOnLoad(this);
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
