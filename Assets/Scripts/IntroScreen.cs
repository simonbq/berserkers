using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour {
	public float timeToAllowTransition = 1.5f;
	public Texture[] movie;
	public string targetLevel;
	private int current = 0;
	public float loopTime = 2.0f;
	private float timeStamp = 0.0f;
	private Color col = Color.white;
	private float timePerFrame;
	bool locked = false;
	private bool activated = false;
	// Use this for initialization
	void Start () {
		AudioListener.volume = Prefs.GetVolume ();
		audio.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		timePerFrame = loopTime / movie.Length;
		if(!locked && !activated && Time.time > timeToAllowTransition && Input.anyKey) {
			activated = true;
			StartCoroutine(transition());
		}
		current = (int)(Time.time / timePerFrame) % movie.Length;
	}

	void OnGUI()
	{
		if(movie.Length != 0) {
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movie[current]);
		}
	}

	private IEnumerator transition() {
		while(col.r > 0.0f) {
			col.r -= Time.deltaTime;
			col.b -= Time.deltaTime;
			col.g -= Time.deltaTime;
			yield return null;
		}
		// menu.SetActive(true);
		// gameObject.SetActive(false);
		Application.LoadLevel (targetLevel);
	}
}
