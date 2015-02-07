﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment;

public class Menu : MonoBehaviour {
	private MenuState current;
	private Stack<MenuState> history = new Stack<MenuState>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		current.update (this);
	}

	public void setCurrent(MenuState state) {
		current = state;
	}
}
