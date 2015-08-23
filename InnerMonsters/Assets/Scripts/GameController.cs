﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EGameState
{
	Preparation,
	GamePlay,
	GameOver
}

public class GameController : MonoBehaviour 
{
	public CameraMgr CameraManager;
	public Camera MainCamera;

	public GameObject MainMenuPanel;
	public GameObject InGamePanel;

	public Image currentItem = null;
	public Image timer = null;

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
	public List<Thought> ThoughtsPrefabs;

	//[System.NonSerialized]
	public Animation itemContainerAnimation = null;
	public PickableObject CurrentlyPickedUpObject;

	const float FLOOR_HEIGHT = 1.6f;
	const float BUILDING_WIDTH = 8f;

	const int BUILDING_MIN_BASE_LEVEL = -1;
	const int BUILDING_MAX_BASE_LEVEL = 0;

	const int BUILDING_MIN_AMOUNT = 3;
	const int BUILDING_MAX_AMOUNT = 5;

	const int BUILDING_MIN_HEIGHT = 2;
	const int BUILDING_MAX_HEIGHT = 4;

	const float TIME_LEFT = 30f;
	const float TIME_REWARD = 5f;

	const float Z_BEHIND_BUILDING = 1f;

	public float TimeLeft = 0f;
	public float TimePlayed = 0f;
	public int Score = 0;

	public UnityEngine.UI.Text Timer;
	public UnityEngine.UI.Text GameOverText;
	public Image gameOverPanel = null;

	public EGameState CurrentGameState;

	public Building CurrentBuilding
	{
		get{ return CurrentLevel.Buildings[CurrentBuildingIndex]; }
	}
	public Floor CurrentFloor
	{
		get{ return CameraManager.currentFloor; }
	}
	
	void Awake()
	{
		CameraManager.enabled = false;
		InGamePanel.SetActive(false);
		MainMenuPanel.SetActive(true);

		MainCamera.orthographicSize = 2.5f;
	}

	// Use this for initialization
//	void Start () 
//	{
//
//	}
	
	// Update is called once per frame
	void Update() 
	{
		if(CurrentGameState == EGameState.Preparation)
		{
			if(Input.GetKey("space"))
			{
				MainMenuPanel.SetActive(false);
				InGamePanel.SetActive(true);
				StartNewGame();
			}
		}

		if(CurrentGameState == EGameState.GamePlay)
		{
			if(TimeLeft < 0f)
			{
				TimeLeft = 0f;

				GameOverText.gameObject.SetActive(true);
				gameOverPanel.gameObject.SetActive( true );
				GameOverText.text = "GAME OVER\nPathetic lives ruined: " + Score.ToString() +  "\nTime wasted: " + ((int)TimePlayed).ToString() + "s\nPress space to try again";

				CurrentGameState = EGameState.GameOver;
				CameraManager.enabled = false;
				CurrentFloor.Reveal( false );
			}
			else
			{
				TimeLeft -= Time.deltaTime;
				TimePlayed += Time.deltaTime;
			}
//			Timer.text = "Time Left: " + ((int)TimeLeft).ToString() + "s";
			Timer.text = ((int)TimeLeft).ToString();
			timer.fillAmount = TimeLeft / TIME_LEFT;

			if( Input.GetKeyUp("space") )
				ObjectInteraction();
		}

		else if(CurrentGameState == EGameState.GameOver)
		{
			if(Input.GetKey("space"))
			{
				StartNewGame();
			}
		}
	}

	void StartNewGame()
	{
		GenerateLevel();
		
		ResetPlayerState();
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

				newFloor.Init(newBuilding.PatternColor, j == buildingHeight - 1, j + newBuilding.BaseLevel == 0, j + newBuilding.BaseLevel < 0);
				
				newFloor.transform.SetParent(newBuilding.transform);
				Vector3 floorPos = Vector3.zero;
				floorPos.y = FLOOR_HEIGHT * (newBuilding.BaseLevel + j);
				newFloor.transform.localPosition = floorPos;
				newFloor.transform.localScale = Vector3.one;	

				newBuilding.Floors.Add(newFloor);
				allGeneratedFloors.Add(newFloor);

				// assign vertical neighbours
				if(j > 0)
				{
					newFloor.nextFloors[(int)Dir.S] = newBuilding.Floors[newBuilding.Floors.Count - 2];
					newBuilding.Floors[newBuilding.Floors.Count - 2].nextFloors[(int)Dir.N] = newFloor;
				}
			}

			// assign horizontal neighbours
			if(i > 0)
			{
				newBuilding.GetGroundFloor().nextFloors[(int)Dir.W] = CurrentLevel.Buildings[CurrentLevel.Buildings.Count - 2].GetGroundFloor();
				CurrentLevel.Buildings[CurrentLevel.Buildings.Count - 2].GetGroundFloor().nextFloors[(int)Dir.E] = newBuilding.GetGroundFloor();
			}
			
			// add roof on top of the building
			GameObject newRoof = Instantiate(RoofPrefab);
			newRoof.transform.SetParent(newBuilding.transform);
			Vector3 roofPos = Vector3.zero;
			roofPos.y = FLOOR_HEIGHT * (newBuilding.BaseLevel + buildingHeight) - 0.15f;
			newRoof.transform.localPosition = roofPos;
			newRoof.transform.localScale = Vector3.one;	
			newRoof.GetComponent<Roof>().Init();
		}

		// don't have any items on ground floor of the first building
		allGeneratedFloors.RemoveAt (0 - CurrentLevel.Buildings[0].BaseLevel);

		// now distribute POIs and pickable objects
		int availableFloorsAmount = allGeneratedFloors.Count;
		for(int i = 0; i < availableFloorsAmount; i += 2)
		{
			// first person
			PersonOfInterest person = Instantiate(PeopleOfInterestPrefabs[UnityEngine.Random.Range(0, PeopleOfInterestPrefabs.Count)]);
			person.Init();
			
			int selectedFloorIndex = UnityEngine.Random.Range(0, allGeneratedFloors.Count);
			allGeneratedFloors[selectedFloorIndex].Person = person;

			person.transform.SetParent(allGeneratedFloors[selectedFloorIndex].transform);
			person.transform.localPosition = Vector3.zero;
			person.transform.localScale = Vector3.one;

			// then select his thoughts
			List<Thought> ApplicableThoughts = new List<Thought>();
			foreach(Thought t in ThoughtsPrefabs)
			{
				if(t.CanBeAppliedToCharacter(person))
					ApplicableThoughts.Add(t);
			}
			Thought thought = Instantiate(ApplicableThoughts[UnityEngine.Random.Range(0, ApplicableThoughts.Count)]);
			person.CurrentThought = thought;

			allGeneratedFloors.RemoveAt(selectedFloorIndex);
			
			if(allGeneratedFloors.Count <= 0)
				return;

			// then corresponding pickable object
			PickableObject pickable = Instantiate(person.CurrentThought.ContraryObjects[UnityEngine.Random.Range(0, person.CurrentThought.ContraryObjects.Count)]);

			selectedFloorIndex = UnityEngine.Random.Range(0, allGeneratedFloors.Count);
			PutObjectOnFloor( pickable, allGeneratedFloors[selectedFloorIndex] );

			allGeneratedFloors.RemoveAt(selectedFloorIndex);
		}
	}

	void ResetPlayerState()
	{
		CurrentBuildingIndex = 0;
		CurrentFloorIndex = 0 - CurrentBuilding.BaseLevel;

		CameraManager.currentFloor = CurrentBuilding.Floors[CurrentFloorIndex];
		CameraManager.Snap();

		TimeLeft = TIME_LEFT;
		TimePlayed = 0f;
		Score = 0;

		CurrentGameState = EGameState.GamePlay;
		CameraManager.enabled = true;
		CameraManager.UpdateArrows();

		RemoveObjectOfHand();
		CurrentFloor.Reveal( true );

		GameOverText.gameObject.SetActive( false );
		gameOverPanel.gameObject.SetActive( false );
	}

	void ClearLevel()
	{
		CurrentLevel.Buildings.Clear();

		foreach(Transform child in LevelParent)
		{
			Destroy(child.gameObject);
		}
	}

	void ObjectInteraction()
	{
		if(CurrentlyPickedUpObject != null)
			DropObject();
		else
			PutObjectOnHand();
	}

	void PutObjectOnHand()
	{
		if( CurrentFloor.Pickable != null )
		{
			CurrentlyPickedUpObject = CurrentFloor.Pickable;
			currentItem.color = new Color( currentItem.color.r, currentItem.color.g, currentItem.color.b, 1.0f );
			currentItem.sprite = CurrentlyPickedUpObject.GetComponentInChildren<SpriteRenderer>().sprite;
			CurrentlyPickedUpObject.transform.localPosition = Vector3.forward * Z_BEHIND_BUILDING;
			CurrentFloor.Pickable = null;
			itemContainerAnimation.Play();
		}
	}

	void RemoveObjectOfHand()
	{
		CurrentlyPickedUpObject = null;
		currentItem.color = new Color( currentItem.color.r, currentItem.color.g, currentItem.color.b, 0.0f );
		currentItem.sprite = null;
		itemContainerAnimation.Stop();
	}

	void PutObjectOnFloor( PickableObject pickable, Floor floor )
	{
		floor.Pickable = pickable;
		pickable.transform.SetParent( floor.transform );
		pickable.transform.localPosition = Vector3.zero;
	}

	void DropObject()
	{
		if( CurrentlyPickedUpObject != null )
		{
			if( CurrentFloor.Pickable != null )
			{
				PickableObject temp = CurrentlyPickedUpObject;
				PutObjectOnHand();
				PutObjectOnFloor( temp, CurrentFloor );
			}
			else
			{
				if( CurrentFloor.Person != null )
				{
					InteractWithPerson();
				}
				else
				{
					PutObjectOnFloor( CurrentlyPickedUpObject, CurrentFloor );
					RemoveObjectOfHand();
				}
			}
		}
	}

	void InteractWithPerson()
	{
		if (CurrentlyPickedUpObject != null && CurrentFloor.Person != null)
		{
			if(CurrentlyPickedUpObject.CanBeUsedOn(CurrentFloor.Person))
			{
				CurrentFloor.Person.UseObjectOn(CurrentlyPickedUpObject);
				CurrentFloor.Deactivate();

				RemoveObjectOfHand();

				TimeLeft += TIME_REWARD;
				Score++;
			}
		}
	}
}
