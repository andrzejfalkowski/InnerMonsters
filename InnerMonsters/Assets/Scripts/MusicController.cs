using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EMusicType
{
	Gameplay,
	GameOver,
	Bullshit,
	FastGameplay,
	TutorialIntro,
	MoreBullshit,
	AbsoluteBullshit,
	TitleScreen,
	OptionsCredits,
	None
}

public class MusicController : MonoBehaviour 
{
//	#1 - gameplay
//	#2 - game over
//	#3 - bullshit
//	#4 - fast gameplay
//	#5 - tutorial / intro
//	#6 - more bullshit
//	#7 - absolute bullshit
//	#8 - title screen
//	#9 - options / credits

	public List<AudioClip> MusicList;
	public AudioSource MusicSource;
	public EMusicType CurrentTrack = EMusicType.None;

	public void PlayMusic(EMusicType music)
	{
		if(CurrentTrack != music)
		{
			CurrentTrack = music;
			MusicSource.clip = MusicList[(int)music];
			MusicSource.Play();
		}
	}
	
}
