using UnityEngine;
using System.Collections;

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
}
