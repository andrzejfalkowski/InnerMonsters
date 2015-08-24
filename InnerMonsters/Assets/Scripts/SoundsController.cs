using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ESoundType
{
	NewGame,
	PickUp,
	Drop,
	Interact,
	FailInteract,
	MoreBuildings,
	SwipeDown,
	SwipeUp,
	SwipeSide,
	GameOver,
	TickTock
}

public class SoundsController : MonoBehaviour 
{
//	- new game/retry stinger
//		- [Wut is dis / Pick / Pickup / Idk] pickup object
//			- [Drop / Cancel] drop object
//			- [Insert] use item on person
//			- [Fail] cannot use item on this person
//			- more buildings unlocked
//			- [Swipe down/up/side] floor/building transition swoosh
//			- game over stinger
//			- [Sth] 10 seconds left tick-tock

	public List<AudioClip> SoundList;
	public AudioSource SoundSource;

	public void PlaySound(ESoundType sfx)
	{
		SoundSource.clip = SoundList[(int)sfx];

		// volume
		switch(sfx)
		{
			case ESoundType.SwipeDown:
			case ESoundType.SwipeUp:
			case ESoundType.SwipeSide:
				SoundSource.volume = 0.2f;
				break;
			default:
				SoundSource.volume = 1f;
				break;
		}

		SoundSource.Play();
	}

}
