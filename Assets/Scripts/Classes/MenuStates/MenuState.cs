using UnityEngine;
using System.Collections;

public abstract class MenuState {

	public abstract void update(MenuBase m);

	public abstract void render();
}
