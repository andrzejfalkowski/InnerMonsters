using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPickableObjectType
{
	Vodka,
	Whisky,
	DemonPainting,
	Pentagram,
	Cat,
	VacuumCleaner,
	Dog,
	Mouse,
	Burglar,
	Police,
	Bug,
	Cocaine,
	Fire
}

public class PickableObject : MonoBehaviour 
{
	public EPickableObjectType Type;

	// list of thought/object interactions
	public static List<Interaction> Interactions = 
		new List<Interaction>()
	{
		new Interaction(EThoughtType.Water, EPickableObjectType.Vodka),
		new Interaction(EThoughtType.Water, EPickableObjectType.Whisky),
		new Interaction(EThoughtType.Angels, EPickableObjectType.DemonPainting),
		new Interaction(EThoughtType.Angels, EPickableObjectType.Pentagram),
		new Interaction(EThoughtType.Dog, EPickableObjectType.Cat),
		new Interaction(EThoughtType.Dog, EPickableObjectType.VacuumCleaner),
		new Interaction(EThoughtType.Cat, EPickableObjectType.Dog),
		new Interaction(EThoughtType.RedHeels, EPickableObjectType.Mouse),
		new Interaction(EThoughtType.Burglar, EPickableObjectType.Police),
		new Interaction(EThoughtType.Police, EPickableObjectType.Burglar),
		new Interaction(EThoughtType.Lilly, EPickableObjectType.Bug),
		new Interaction(EThoughtType.BusinessCase, EPickableObjectType.Cocaine),
		new Interaction(EThoughtType.Fireman, EPickableObjectType.Fire)
	};

	// some objects can be used only on specific characters
	// if exclusion is not specified, then assume we can use it on any character
	public static Dictionary<EPickableObjectType, List<ECharacterType>> ObjectCharacterExclusions = 
		new Dictionary<EPickableObjectType, List<ECharacterType>>()
	{
		{ EPickableObjectType.Cat, new List<ECharacterType>(){ ECharacterType.Dog } },
		{ EPickableObjectType.VacuumCleaner, new List<ECharacterType>(){ ECharacterType.Dog } },
		{ EPickableObjectType.Dog, new List<ECharacterType>(){ ECharacterType.Cat } },
		{ EPickableObjectType.Mouse, new List<ECharacterType>(){ ECharacterType.Woman } },
	};

	public bool IsContraryForThought(EThoughtType thought)
	{
		foreach(Interaction interaction in Interactions)
		{
			if(interaction.PickableObject == Type && interaction.ThoughtType == thought)
				return true;
		}
		return false;
	}

	public bool CanBeUsedOn(PersonOfInterest person)
	{
		return IsContraryForThought(person.ThoughtType) && CanBeUsedOnCharacterType(person.CharacterType);
	}

	bool CanBeUsedOnCharacterType(ECharacterType character)
	{
		return !ObjectCharacterExclusions.ContainsKey(Type) || ObjectCharacterExclusions [Type].Contains(character);
	}

	public void PickUpObject()
	{
		// TODO
	}
}
