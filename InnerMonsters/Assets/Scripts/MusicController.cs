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
	private Fade fade = Fade.NONE;
	const float TRANSITION_TIME = 0.25f;

	public List<AudioClip> MusicList;
	public AudioSource MusicSource;
	public EMusicType CurrentTrack = EMusicType.None;

	public void PlayMusic(EMusicType music)
	{
		if(CurrentTrack != music)
		{
			CurrentTrack = music;
			if(CurrentTrack == EMusicType.None)
			{
				fade = Fade.IN;
				MusicSource.clip = MusicList[(int)music];
				MusicSource.Play();
			}
			else
				fade = Fade.OUT;
		}
	}

	void FixedUpdate()
	{
		switch( fade )
		{
			case Fade.IN:
			{
				float newVolume = MusicSource.volume + Time.fixedDeltaTime / TRANSITION_TIME;
				
				if( newVolume < 1.0f ) 
					UpdateMusicVolume( newVolume );
				else
					StopFade( 1.0f, Fade.NONE );
				break;
			}
				
			case Fade.OUT:
			{
				float newVolume = MusicSource.volume - Time.fixedDeltaTime / TRANSITION_TIME;
				
				if( newVolume > 0.0f ) 
					UpdateMusicVolume( newVolume );
				else
				{
					StopFade( 0.0f, Fade.IN );
					MusicSource.clip = MusicList[(int)CurrentTrack];
					MusicSource.Play();
				}
				break;
			}
		}
	}

	void StopFade( float alpha, Fade nextFadeState )
	{
		UpdateMusicVolume( alpha );
		fade = nextFadeState;
	}

	void UpdateMusicVolume( float value )
	{
		MusicSource.volume = value;
	}
}
