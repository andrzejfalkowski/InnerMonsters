using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECharacterType
{
	Man,
	Woman,
	Kid,
	Dog,
	Cat,
}

public class PersonOfInterest : MonoBehaviour 
{
	public ECharacterType CharacterType;
	public Thought CurrentThought;

	public SpriteRenderer FaceRenderer;
	public Sprite HappyFace;
	public Sprite SadFace;

	public bool Ruined = false;
	//public EThoughtType ThoughtType;

	// TODO: some POIs can be also contrary object for antoher POI, e.g. cat and dog can both be used against each other
	public PickableObject OwnPickableObject;

	
	public void Init()
	{
		Ruined = false;
		UpdateFace();
	}

	public void UpdateFace()
	{
		if(FaceRenderer != null)
			FaceRenderer.sprite = (Ruined ? SadFace : HappyFace);
	}
	
	public void UseObjectOn(PickableObject pickable)
	{
		// TODO: handling people getting drunk, etc.
	}
}
