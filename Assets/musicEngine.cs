using UnityEngine;
using System.Collections;

public class musicEngine : MonoBehaviour {
	
	public AudioClip[] music_0_Clips;
	public AudioClip[] music_1_Clips;
	
	int progression = 0;
	
	int musicCurrent = 0;
	int musicPrevious = 0;
	
	int levelCurrent = 0;
	int levelPrevious = 0;
	
	System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
	
	public void musicSetVolume (float volume)
	{
		audio.volume = volume;
	}
	
	void OnLevelWasLoaded()
	{
		if (Application.loadedLevel != levelPrevious)
		{
			audio.Stop();
			levelPrevious = Application.loadedLevel;
			
			progression = 0;
			musicPrevious = 0;
			musicCurrent = 0;
		}
	}
	
	void Start ()
	{
		stopwatch.Start();
	}
	
	void Update ()
	{
		if (Application.loadedLevelName == "Lobby")
		{
			if (stopwatch.ElapsedMilliseconds >= music_0_Clips[musicCurrent].length*1000-2000 || progression == 0)
			{
				if (progression == 0)
				{
					musicCurrent = 0;
					audio.PlayOneShot (music_0_Clips[musicCurrent]);
					progression = 1;
				}
				
				else if (progression == 1)
				{
					musicCurrent = 1;
					audio.PlayOneShot (music_0_Clips[1]);
				}
				
				stopwatch.Reset();
				stopwatch.Start();
			}
		}
		
		else if (Application.loadedLevelName == "Level")
		{
			if (stopwatch.ElapsedMilliseconds >= music_1_Clips[musicCurrent].length*1000-2000 || progression == 0)
			{
				if (progression == 0)
				{
					audio.PlayOneShot (music_1_Clips[0]);
					
					musicCurrent = 0;
					progression = 1;
				}
				
				else if (progression == 1)
				{
					while(musicCurrent == musicPrevious)
					{
						musicCurrent = Random.Range (1,8);
					}
					
					musicPrevious = musicCurrent;
					
					audio.PlayOneShot (music_1_Clips[musicCurrent]);
				}
				
				stopwatch.Reset();
				stopwatch.Start();
			}
		}
	}
}
 