using UnityEngine;
using System.Collections;

public class musicEngine : MonoBehaviour {

	public AudioClip[] music_0_Clips;
	public AudioClip[] music_1_Clips;
	
	int progression = 0;

	int randomCurrent = 0;
	int randomPrevious = 0;

	int levelCurrent = 0;

	System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

	public void musicSetVolume (float volume)
	{
		audio.volume = volume;
	}

	public void musicSetLevel(int level)
	{
		if (level != levelCurrent)
		{
			audio.Stop();
			progression = 0;
			levelCurrent = level;
		}
	}

	void Start ()
	{
		stopwatch.Start();
	}

	void Update ()
	{
		if (levelCurrent == 0)
		{
			if (stopwatch.ElapsedMilliseconds >= music_0_Clips[0].length*1000 || progression == 0)
			{
				audio.PlayOneShot(music_0_Clips[0]);

				progression = 1;

				stopwatch.Reset();
				stopwatch.Start();
			}
		}

		else if (levelCurrent == 1)
		{
			if (stopwatch.ElapsedMilliseconds >= music_1_Clips[randomPrevious].length*1000-2000 || progression == 0)
			{
				if (progression == 0)
				{
					audio.PlayOneShot (music_1_Clips[0]);
					
					progression = 1;
				}
				
				else if (progression == 1)
				{
					while(randomCurrent == randomPrevious)
					{
						randomCurrent = Random.Range (1,4);
					}
					
					randomPrevious = randomCurrent;
					
					audio.PlayOneShot (music_1_Clips[randomCurrent]);
				}
				
				stopwatch.Reset();
				stopwatch.Start();
			}
		}
	}
}
