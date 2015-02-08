using UnityEngine;
using System.Collections;

public abstract class MenuState {

	public abstract void update(Menu m);

	public abstract void render();
}
