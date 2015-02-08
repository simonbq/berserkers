﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment;

public class Menu : MonoBehaviour {
	public static readonly Vector2 SCREEN_SIZE = new Vector2 (1920, 1080);

	private MenuState current;
	private Stack<MenuState> history = new Stack<MenuState>();
	public GUISkin skin;
	private Matrix4x4 matrix;
	private Vector3 scale = new Vector3();
	// Use this for initialization
	void Awake () {
		current = MenuStates.MAIN;

	}
	
	// Update is called once per frame
	void Update () {
		float width = Screen.width / SCREEN_SIZE.x;
		float height = Screen.height / SCREEN_SIZE.y;
		scale = new Vector3(width, height, 0);
		matrix = Matrix4x4.TRS (scale, Quaternion.identity, new Vector3(scale.x, scale.y, 1));
	}

	void OnGUI() {
		GUI.matrix = matrix;
		if(skin != null) {
			GUI.skin = skin;
		}
		current.update (this);
	}

	public void setCurrent(MenuState state) {
		history.Push (current);
		current = state;
	}

	public void goBack() {
		current = history.Pop ();
	}
}