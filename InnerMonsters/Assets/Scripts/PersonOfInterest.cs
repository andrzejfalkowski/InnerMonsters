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

public enum ECharacterType
{
	Man,
	Woman,
	Dog,
	Cat,
}

public class PersonOfInterest : MonoBehaviour 
{
	public ECharacterType CharacterType;
	public EThoughtType ThoughtType;

	// TODO: some POIs can be also contrary object for antoher POI, e.g. cat and dog can both be used against each other
	public PickableObject OwnPickableObject;

	public void UseObjectOn(PickableObject pickable)
	{
		// TODO: handling people getting drunk, etc.
	}

	public List<EPickableObjectType> GetListOfCorrespondingPickables()
	{
		List<EPickableObjectType> list = new List<EPickableObjectType>();
		foreach(Interaction interaction in PickableObject.Interactions)
		{
			if(interaction.ThoughtType == ThoughtType)
			{
				list.Add(interaction.PickableObject);
			}
		}
		return list;
	}
	
}
