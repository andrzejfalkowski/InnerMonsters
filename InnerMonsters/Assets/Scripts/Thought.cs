using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EThoughtType
{
	Coolness,
	Engagement,
	Fire,
	Food,
	Lungs,
	Money,
	Parents,
	Pregnant,
	Religion,
	Taxi,
	Thirst,
	VideoGame,
	Tired
}

public class Thought : MonoBehaviour 
{
	public EThoughtType ThoughtType;

	public List<string> BubbleSpeeches;
	public List<PersonOfInterest> SpecificCharactersOnly;

	public List<PickableObject> ContraryObjects;
	public List<string> TraumaSpeeches;

	public Sprite Symbol;

	public bool CanBeAppliedToCharacter(PersonOfInterest person)
	{
		if(SpecificCharactersOnly.Count == 0)
			return true;

		foreach(PersonOfInterest p in SpecificCharactersOnly)
		{
			if(p.CharacterType == person.CharacterType)
				return true;
		}
		return false;
	}
}
