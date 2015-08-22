using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	public Transform LevelParent;

	[System.NonSerialized]
	public Level CurrentLevel;	
	[System.NonSerialized]
	public int CurrentBuildingIndex = 0;
	[System.NonSerialized]
	public int CurrentFloorIndex = 0;

	[System.NonSerialized]
	public PickableObject CurrentlyPickedUpObject;
	
	public Building CurrentBuilding
	{
		get{ return CurrentLevel.Buildings[CurrentBuildingIndex]; }
	}
	public Floor CurrentFloor
	{
		get{ return CurrentBuilding.Floors[CurrentFloorIndex]; }
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update() 
	{
	
	}

	void GenerateLevel()
	{
		ClearLevel();

		// TODO: generate level
	}

	void ClearLevel()
	{
		foreach(Transform child in LevelParent)
		{
			Destroy(child);
		}
	}

	void PickUpObject()
	{
		if (CurrentFloor.Pickable != null)
		{
			if(CurrentlyPickedUpObject != null)
				DropObject();
			else
			{
				CurrentlyPickedUpObject = CurrentFloor.Pickable;
				CurrentFloor.Pickable.PickUpObject();
			}
		}

	}

	void DropObject()
	{
		if (CurrentlyPickedUpObject != null)
		{
			if(CurrentFloor.Pickable != null)
			{
				PickableObject temp = CurrentlyPickedUpObject;
				CurrentlyPickedUpObject = CurrentFloor.Pickable;
				CurrentFloor.Pickable = temp;
				CurrentlyPickedUpObject = temp;
			}
			else
			{
				CurrentFloor.Pickable = CurrentlyPickedUpObject;
				CurrentlyPickedUpObject = null;
			}
		}
	}

	void InteractWithPerson()
	{
		if (CurrentlyPickedUpObject != null && CurrentFloor.Person != null)
		{
			if(CurrentlyPickedUpObject.CanBeUsedOn(CurrentFloor.Person))
				CurrentFloor.Person.UseObjectOn(CurrentlyPickedUpObject);
		}
	}
}
