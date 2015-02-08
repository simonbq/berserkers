using UnityEngine;
using System.Collections;

public class Killstreaks {
	private float lastKillTime = 0.0f;
	private int killsInARow = 0;
	private int fastKillsInARow = 0;

	private const float fastKillTime = 1.5f;

	public void AddKill() {
		if (lastKillTime + fastKillTime < Time.time) { //failed to kill fast
			fastKillsInARow = 0;
		}
		fastKillsInARow++;
		killsInARow++;
		lastKillTime = Time.time;
	}

	public int GetFastKills() {
		return fastKillsInARow;
	}

	public int GetKills() {
		return killsInARow;
	}

	public void Died() {
		fastKillsInARow = 0;
		killsInARow = 0;
		lastKillTime = 0.0f;
	}
}