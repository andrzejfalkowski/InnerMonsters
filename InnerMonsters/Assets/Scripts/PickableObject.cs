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
	
	public bool IsContraryForThought(Thought thought)
	{
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
