using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour {
	public float timeToAllowTransition = 1.5f;
	public GameObject menu;
	public MovieTexture movie;
	private Color col = Color.white;
	bool locked = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!locked && Time.time > timeToAllowTransition && Input.anyKey) {
			StartCoroutine(transition);
		}
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), movie);
	}

	private IEnumerator transition() {
		while(col.r > 0.0f) {
			col.r -= Time.deltaTime;
			col.b -= Time.deltaTime;
			col.g -= Time.deltaTime;
			yield return null;
		}
		menu.SetActive(true);
		gameObject.SetActive(false);
	}
}
