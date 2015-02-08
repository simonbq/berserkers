using UnityEngine;
using System.Collections;

public class soundList : MonoBehaviour {
	public AudioClip[] audioClips;
	public bool playOnAwake = false;
	// Use this for initialization
	void Awake () {
		if(playOnAwake) PlayRandom();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(int index) {
		if (index >= audioClips.Length || index < 0) {
			Debug.LogError ("invalid index : soundList.Play(int index)");		
		} 

		else {
			audio.clip = audioClips [index];
			audio.Play();
		}
	}
	public void Play() {
		Play (0);
	}

	public void PlayRandom() {
		if (audioClips.Length < 1) {
			Debug.LogError ("there are no audio clips in this list");
		}
		else {
		audio.clip = audioClips [Random.Range (0, audioClips.Length)];
		audio.Play ();
		}
	}
}