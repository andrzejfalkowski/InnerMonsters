using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	public Transform LevelParent;
	public Level CurrentLevel;	
	[System.NonSerialized]
	public int CurrentBuildingIndex = 0;
	[System.NonSerialized]
	public int CurrentFloorIndex = 0;
	
	public GameObject BuildingPrefab;
	public GameObject FloorPrefab;
	public GameObject RoofPrefab;

	public List<PersonOfInterest> PeopleOfInterestPrefabs;
	public List<PickableObject> PickableObjectsPrefabs;

	[System.NonSerialized]
	public PickableObject CurrentlyPickedUpObject;

	const float FLOOR_HEIGHT = 1.6f;
	const float BUILDING_WIDTH = 8f;

	const int BUILDING_MIN_BASE_LEVEL = -2;
	const int BUILDING_MAX_BASE_LEVEL = 0;

	const int BUILDING_MIN_AMOUNT = 3;
	const int BUILDING_MAX_AMOUNT = 5;

	const int BUILDING_MIN_HEIGHT = 2;
	const int BUILDING_MAX_HEIGHT = 4;

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
		GenerateLevel ();
	}
	
	// Update is called once per frame
	void Update() 
	{
	
	}

	void GenerateLevel()
	{
		ClearLevel();

		// first create building and floors
		List<Floor> allGeneratedFloors = new List<Floor>();
		int amountOfBuildings = UnityEngine.Random.Range (BUILDING_MIN_AMOUNT, BUILDING_MAX_AMOUNT);
		for(int i = 0; i < amountOfBuildings; i++)
		{
			GameObject newBuildingObject = Instantiate(BuildingPrefab);
			Building newBuilding = newBuildingObject.GetComponent<Building>();

			newBuilding.transform.SetParent(LevelParent);
			Vector3 buildingPos = Vector3.zero;
			buildingPos.x = BUILDING_WIDTH * i;
			newBuilding.transform.localPosition = buildingPos;
			newBuilding.transform.localScale = Vector3.one;

			int buildingHeight =  UnityEngine.Random.Range(BUILDING_MIN_HEIGHT, BUILDING_MAX_HEIGHT + 1);
			newBuilding.BaseLevel = UnityEngine.Random.Range(BUILDING_MIN_BASE_LEVEL, BUILDING_MAX_BASE_LEVEL + 1);

			// make sure each building is at least above the ground, we don't want any bunkers
			if(buildingHeight + newBuilding.BaseLevel < BUILDING_MIN_HEIGHT)
				buildingHeight = BUILDING_MIN_HEIGHT - newBuilding.BaseLevel;

			// select facade
			newBuilding.Init();

			CurrentLevel.Buildings.Add(newBuilding);

			for(int j = 0; j < buildingHeight; j++)
			{
				GameObject newFloorObject = Instantiate(FloorPrefab);
				Floor newFloor = newFloorObject.GetComponent<Floor>();

				newFloor.Init(newBuilding.FacadeType, j == buildingHeight - 1, j + newBuilding.BaseLevel == 0, j + newBuilding.BaseLevel < 0);
				
				newFloor.transform.SetParent(newBuilding.transform);
				Vector3 floorPos = Vector3.zero;
				floorPos.y = FLOOR_HEIGHT * (newBuilding.BaseLevel + j);
				newFloor.transform.localPosition = floorPos;
				newFloor.transform.localScale = Vector3.one;	

				newBuilding.Floors.Add(newFloor);
				allGeneratedFloors.Add(newFloor);
			}

			// add roof on top of the building
			GameObject newRoof = Instantiate(RoofPrefab);
			newRoof.transform.SetParent(newBuilding.transform);
			Vector3 roofPos = Vector3.zero;
			roofPos.y = FLOOR_HEIGHT * (newBuilding.BaseLevel + buildingHeight);
			newRoof.transform.localPosition = roofPos;
			newRoof.transform.localScale = Vector3.one;				
		}

		// now distribute POIs and pickable objects
		int availableFloorsAmount = allGeneratedFloors.Count;
		int peopleOfInterestAmount = PeopleOfInterestPrefabs.Count;
		int pickableObjectsAmount = 0;
		for(int i = 0; i < availableFloorsAmount; i += 2)
		{
			// first person
			PersonOfInterest person = Instantiate(PeopleOfInterestPrefabs[UnityEngine.Random.Range(0, peopleOfInterestAmount)]);

			int selectedFloorIndex = UnityEngine.Random.Range(0, allGeneratedFloors.Count);
			allGeneratedFloors[selectedFloorIndex].Person = person;

			person.transform.SetParent(allGeneratedFloors[selectedFloorIndex].transform);
			person.transform.localPosition = Vector3.zero;
			person.transform.localScale = Vector3.one;

			allGeneratedFloors.RemoveAt(selectedFloorIndex);

			if(allGeneratedFloors.Count <= 0)
				return;

			// then corresponding pickable object
			List<EPickableObjectType> applicablePickables = person.GetListOfCorrespondingPickables();
			List<PickableObject> applicablePickablePrefabs = new List<PickableObject>();
			foreach(EPickableObjectType pickableType in applicablePickables)
			{
				foreach(PickableObject pickablePrefab in PickableObjectsPrefabs)
				{
					if(pickableType == pickablePrefab.Type)
						applicablePickablePrefabs.Add(pickablePrefab);
				}
			}
			pickableObjectsAmount = applicablePickablePrefabs.Count;

			PickableObject pickable = Instantiate(PickableObjectsPrefabs[UnityEngine.Random.Range(0, pickableObjectsAmount)]);

			selectedFloorIndex = UnityEngine.Random.Range(0, allGeneratedFloors.Count);
			allGeneratedFloors[selectedFloorIndex].Pickable = pickable;

			pickable.transform.SetParent(allGeneratedFloors[selectedFloorIndex].transform);
			pickable.transform.localPosition = Vector3.zero;
			pickable.transform.localScale = Vector3.one;

			allGeneratedFloors.RemoveAt(selectedFloorIndex);
		}
	}

	void ClearLevel()
	{
		CurrentLevel.Buildings.Clear();

		foreach(Transform child in LevelParent)
		{
			Destroy(child.gameObject);
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
