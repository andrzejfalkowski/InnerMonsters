using UnityEngine;
using System.Collections;

public class Interaction
{
	public EThoughtType ThoughtType;
	public EPickableObjectType PickableObject;
	
	public Interaction(EThoughtType thoughtType, EPickableObjectType pickableObject)
	{
		ThoughtType = thoughtType;
		PickableObject = pickableObject;
	}
}
