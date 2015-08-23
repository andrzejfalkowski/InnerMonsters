using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EThoughtType
{
	Water,
	Angels,
	Dog,
	Cat,
	RedHeels,
	Burglar,
	Police,
	Lilly,
	BusinessCase,
	Fireman
}

public class Thought : MonoBehaviour 
{
	public List<string> BubbleSpeeches;
	public List<PersonOfInterest> SpecificCharactersOnly;

	public List<PickableObject> ContraryObjects;
	public List<string> TraumaSpeeches;

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
