using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MenuBase : MonoBehaviour {
	public static readonly Vector2 SCREEN_SIZE = new Vector2 (1920, 1080);
	protected Vector2 screenScale = Vector2.one;
	public Vector2 screen_scale { get { return screenScale; } }
	protected MenuState current { get; private set; }
	private Stack<MenuState> history = new Stack<MenuState>();



	public void setCurrent(MenuState state) {
		if(current != null)
			history.Push (current);
		current = state;
	}
	
	public void goBack() {
		if(history.Count > 0)
			current = history.Pop ();
	}
}
