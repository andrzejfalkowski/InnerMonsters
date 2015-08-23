using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPickableObjectType
{
	Vodka,
	Whisky,
	DemonPainting,
	Pentagram,
	Crowbar,
	BurglarMask,
	Matches,
	Gamepad,
	Burger,
	Steak,
	Cigarettes,
	Poker,
	Cocainum,
	Razor,
	CarKeys
}

public class PickableObject : MonoBehaviour 
{
	public EPickableObjectType Type;
	
	public bool IsContraryForThought(Thought thought)
	{
		if(thought == null)
		   return false;

		foreach(PickableObject pickable in thought.ContraryObjects)
		{
			if(pickable.Type == Type)
				return true;
		}
		return false;
	}

	public bool CanBeUsedOn(PersonOfInterest person)
	{
		return IsContraryForThought(person.CurrentThought);
	}
}
