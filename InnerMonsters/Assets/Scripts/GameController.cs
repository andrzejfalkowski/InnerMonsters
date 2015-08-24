using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EGameState
{
	MainMenu,
	GamePlay,
	GameOver,
	Transition
}

public enum ETutorialProgress
{
	ArrowKeys,
	PickUp,
	Interact,
	Timer,
	Finished,
	Preparation
}

public class GameController : MonoBehaviour 
{
	public CameraMgr CameraManager;
	public Camera MainCamera;

	public MusicController Music;
	public SoundsController Sounds;
	public TransitionController Transition;

	public GameObject MainMenuPanel;
	public GameObject InGamePanel;

	public Image currentItem = null;
	public Image timer = null;
	public GameObject timerContainer = null;

	public Transform LevelParent;
	public Level CurrentLevel;	
	[System.NonSerialized]
	public int CurrentBuildingIndex = 0;
	[System.NonSerialized]
	public int CurrentFloorIndex = 0;
	[System.NonSerialized]
	public int MaxBuildingIndex = 0;
	
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
	const int BUILDING_MAX_HEIGHT = 5;

	const float TIME_LEFT = 30f;
	const float TIME_REWARD = 10f;

	const float Z_BEHIND_BUILDING = 1f;

	public float TimeLeft = 0f;
	public float TimePlayed = 0f;
	public int Score = 0;
	public Animation timerAnimation = null;
	public float animateClockWhenSecondsLeft = 15.0f;

	public float MAX_CLOCK_ANIMATION_SPEED = 3.0f;

	public float TIME_LEFT_FAST_MUSIC_TRESHOLD = 10f;

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

	public int AmountOfMatchesLeft = 0;
	const int MORE_BUILDINGS_TRESHOLD = 2;

	private List<EThoughtType> thoughtsGeneratedDuringThisRound = new List<EThoughtType>();

	public bool IsFirstGame = true;

	public ETutorialProgress TutorialProgress = ETutorialProgress.ArrowKeys;

	public List<UnityEngine.UI.Text> TutorialTexts;

	public UnityEngine.UI.Text MoreBuildingsIndicator;
	bool moreBuildingsIndicatorShown = false;
	float moreBuildingsIndicatorTimer = 0f;
	const float MORE_BUILDINGS_TIME = 3.0f;

	public Transform ParallaxesParent;
	public GameObject GroundPrefab;
	private float minGroundX = 0f;
	private float maxGroundX = 0f;

	void Awake()
	{
		CameraManager.enabled = false;
		InGamePanel.SetActive(false);
		MainMenuPanel.SetActive(true);

		MainCamera.orthographicSize = 2.5f;

		Music.PlayMusic(EMusicType.TitleScreen);
	}

	// Use this for initialization
//	void Start () 
//	{
//
//	}
	
	// Update is called once per frame
	void Update() 
	{
		if(CurrentGameState == EGameState.MainMenu)
		{
			if(Input.GetKey("space"))
			{
				CurrentGameState = EGameState.Transition;
				Transition.StartFade(
					()=>{
						MainMenuPanel.SetActive(false);
						InGamePanel.SetActive(true);
						StartNewGame();
					},
					null
				);
			}
			else if(Input.GetKey(KeyCode.Escape))
			{
				CurrentGameState = EGameState.Transition;
				Transition.StartFade(
					()=>{
					Application.Quit();
				},
				null
				);
			}
		}
		
		if(CurrentGameState == EGameState.GamePlay)
		{
			if(TimeLeft < 0f)
			{
				TimeLeft = 0f;

				GameOverText.gameObject.SetActive(true);
				gameOverPanel.gameObject.SetActive( true );
				GameOverText.text = "GAME OVER\nPathetic lives ruined: " + Score.ToString() +  "\nTime wasted: " + ((int)TimePlayed).ToString() + "s\nPress SPACE to try again";

				MoreBuildingsIndicator.gameObject.SetActive(false);
				HideTutorial();

				// if tutorial wasn't finished, reset progress
				CurrentGameState = EGameState.GameOver;
				CameraManager.enabled = false;
				CurrentFloor.Reveal( false );
				timerContainer.SetActive( false );
				timerAnimation.Stop();

				Sounds.PlaySound(ESoundType.GameOver);
				Music.PlayMusic(EMusicType.GameOver);
			}
			else
			{
				if(!IsFirstGame)
					TimeLeft -= Time.deltaTime;
				TimePlayed += Time.deltaTime;

				if( TimeLeft < animateClockWhenSecondsLeft )
				{
					timerAnimation.Play();
					timerAnimation["ClockBeat"].speed = MAX_CLOCK_ANIMATION_SPEED * (1 - (TimeLeft / animateClockWhenSecondsLeft) );
				}

				if(TimeLeft < TIME_LEFT_FAST_MUSIC_TRESHOLD)
				{
					Music.PlayMusic(EMusicType.FastGameplay);
				}
				else
				{
					Music.PlayMusic(EMusicType.Gameplay);
				}

				if(moreBuildingsIndicatorShown)
				{
					moreBuildingsIndicatorTimer -= Time.deltaTime;
					if( moreBuildingsIndicatorTimer < 0 )
					{
						moreBuildingsIndicatorShown = false;
						MoreBuildingsIndicator.gameObject.SetActive(false);
					}
				}
			}
//			Timer.text = "Time Left: " + ((int)TimeLeft).ToString() + "s";
			Timer.text = ((int)TimeLeft).ToString();
			timer.fillAmount = TimeLeft / TIME_LEFT;

			if( Input.GetKeyUp("space") )
				ObjectInteraction();
			else if(Input.GetKey(KeyCode.Escape))
			{
				CurrentGameState = EGameState.Transition;
				
				Music.PlayMusic(EMusicType.TitleScreen);
				Transition.StartFade(
					()=>{
					CurrentGameState = EGameState.MainMenu;
					MainMenuPanel.SetActive(true);
					InGamePanel.SetActive(false);
				},
				null
				);
			}
		}

		else if(CurrentGameState == EGameState.GameOver)
		{
			if(Input.GetKey("space"))
			{
				CurrentGameState = EGameState.Transition;
				Transition.StartFade(
					()=>{
						StartNewGame();
					},
					null
				);
			}
			else if(Input.GetKey(KeyCode.Escape))
			{
				CurrentGameState = EGameState.Transition;
				
				Music.PlayMusic(EMusicType.TitleScreen);
				Transition.StartFade(
					()=>{
					CurrentGameState = EGameState.MainMenu;
					MainMenuPanel.SetActive(true);
					InGamePanel.SetActive(false);
				},
				null
				);
			}
		}
	}

	void StartNewGame()
	{
		if(IsFirstGame)
			UpdateTutorialState(ETutorialProgress.ArrowKeys);

		AmountOfMatchesLeft = 0;

		GenerateLevel();
		
		ResetPlayerState();

		Music.PlayMusic(EMusicType.Gameplay);
	}

	void GenerateLevel()
	{
		ClearLevel();

		MaxBuildingIndex = 0;

		GenerateNewBuildings(1, 1, true);
	}

	void GenerateNewBuildings(int minAmount, int maxAmount, bool isNewGame = false)
	{
		// first create building and floors
		List<Floor> allGeneratedFloors = new List<Floor>();
		int amountOfBuildings = UnityEngine.Random.Range (minAmount, maxAmount);

		// "tutorial"
		if (isNewGame)
			amountOfBuildings = 1;

		for(int i = 0; i < amountOfBuildings; i++)
		{
			GameObject newBuildingObject = Instantiate(BuildingPrefab);
			Building newBuilding = newBuildingObject.GetComponent<Building>();
			
			newBuilding.transform.SetParent(LevelParent);
			Vector3 buildingPos = Vector3.zero;
			buildingPos.x = BUILDING_WIDTH * (float)(((int)(i + MaxBuildingIndex + 1)) / 2);

			if((i + MaxBuildingIndex) % 2 == 0)
				buildingPos.x *= -1f;

			newBuilding.transform.localPosition = buildingPos;
			newBuilding.transform.localScale = Vector3.one;
			
			int buildingHeight =  UnityEngine.Random.Range(BUILDING_MIN_HEIGHT, BUILDING_MAX_HEIGHT + 1);
			newBuilding.BaseLevel = UnityEngine.Random.Range(BUILDING_MIN_BASE_LEVEL, BUILDING_MAX_BASE_LEVEL + 1);
			
			// make sure each building is at least above the ground, we don't want any bunkers
			if(buildingHeight + newBuilding.BaseLevel < BUILDING_MIN_HEIGHT)
				buildingHeight = BUILDING_MIN_HEIGHT - newBuilding.BaseLevel;

			// "tutorial"
			if(isNewGame)
			{
				newBuilding.BaseLevel = -1;
				buildingHeight = 3;
			}

			// select facade
			newBuilding.Init();

			int buildingsIndex = 0;
			if((i + MaxBuildingIndex) % 2 == 1)
			{
				buildingsIndex = CurrentLevel.Buildings.Count;
				CurrentLevel.Buildings.Add(newBuilding);
			}
			else
			{
				CurrentLevel.Buildings.Insert(0,newBuilding);
			}

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
			if(buildingsIndex > 0)
			{
				newBuilding.GetGroundFloor().nextFloors[(int)Dir.W] = CurrentLevel.Buildings[CurrentLevel.Buildings.Count - 2].GetGroundFloor();
				CurrentLevel.Buildings[CurrentLevel.Buildings.Count - 2].GetGroundFloor().nextFloors[(int)Dir.E] = newBuilding.GetGroundFloor();
			}
			else if(!isNewGame)
			{
				newBuilding.GetGroundFloor().nextFloors[(int)Dir.E] = CurrentLevel.Buildings[buildingsIndex + 1].GetGroundFloor();
				CurrentLevel.Buildings[buildingsIndex + 1].GetGroundFloor().nextFloors[(int)Dir.W] = newBuilding.GetGroundFloor();
			}

			// generate ground if needed
			if(MaxBuildingIndex > maxGroundX / 5f)
			{
				GameObject ground1 = Instantiate(GroundPrefab);
				ground1.transform.SetParent(ParallaxesParent);
				Vector3 ground1Pos = Vector3.zero;
				minGroundX -= 108f;
				ground1Pos.x = minGroundX;
				ground1.transform.localPosition = ground1Pos;
				ground1.transform.localScale = Vector3.one;

				GameObject ground2 = Instantiate(GroundPrefab);
				ground2.transform.SetParent(ParallaxesParent);
				Vector3 ground2Pos = Vector3.zero;
				maxGroundX += 108f;
				ground2Pos.x = maxGroundX;
				ground2.transform.localPosition = ground2Pos;
				ground2.transform.localScale = Vector3.one;
			}

			// add roof on top of the building
			GameObject newRoof = Instantiate(RoofPrefab);
			newRoof.transform.SetParent(newBuilding.transform);
			Vector3 roofPos = Vector3.zero;
			roofPos.y = FLOOR_HEIGHT * (newBuilding.BaseLevel + buildingHeight);
			newRoof.transform.localPosition = roofPos;
			newRoof.transform.localScale = Vector3.one;	
			newRoof.GetComponent<Roof>().Init();
		}

		// we'll need this next time
		MaxBuildingIndex += amountOfBuildings;

		// don't have any items on ground floor of the first building
		if(isNewGame)
			allGeneratedFloors.RemoveAt (0 - CurrentLevel.Buildings[0].BaseLevel);
		
		// now distribute POIs and pickable objects
		int availableFloorsAmount = allGeneratedFloors.Count;
		for(int i = 0; i < availableFloorsAmount; i += 2)
		{
			// first generate thought
			List<Thought> ApplicableThoughts = new List<Thought>();
			foreach(Thought t in ThoughtsPrefabs)
			{
				if(!thoughtsGeneratedDuringThisRound.Contains(t.ThoughtType))
					ApplicableThoughts.Add(t);
			}
			
			// no unique thoguhts available, fallback to any
			if(ApplicableThoughts.Count == 0)
			{
				foreach(Thought t in ThoughtsPrefabs)
				{
					ApplicableThoughts.Add(t);
				}
			}
			
			// OK, allow thoughts to repeat again if all already exist
			if(thoughtsGeneratedDuringThisRound.Count == ThoughtsPrefabs.Count)
			{
				thoughtsGeneratedDuringThisRound.Clear();
			}
			
			Thought thought = Instantiate(ApplicableThoughts[UnityEngine.Random.Range(0, ApplicableThoughts.Count)]);
			thoughtsGeneratedDuringThisRound.Add(thought.ThoughtType);

			// then generate person
			List<PersonOfInterest> ApplicablePeople = new List<PersonOfInterest>();
			foreach(PersonOfInterest p in PeopleOfInterestPrefabs)
			{
				if(thought.CanBeAppliedToCharacter(p))
					ApplicablePeople.Add(p);
			}
			
			PersonOfInterest person = Instantiate(ApplicablePeople[UnityEngine.Random.Range(0, ApplicablePeople.Count)]);
			person.CurrentThought = thought;
			person.Init();
			
			int selectedFloorIndex = UnityEngine.Random.Range(0, allGeneratedFloors.Count);

			// tutorial - always person on 1st, then item on 2nd
			if(isNewGame && IsFirstGame)
			{
				selectedFloorIndex = i;
			}

			allGeneratedFloors[selectedFloorIndex].Person = person;
			
			person.transform.SetParent(allGeneratedFloors[selectedFloorIndex].transform);
			person.transform.localPosition = Vector3.zero;
			person.transform.localScale = Vector3.one;

			allGeneratedFloors.RemoveAt(selectedFloorIndex);
			
			if(allGeneratedFloors.Count <= 0)
				return;

			AmountOfMatchesLeft++;

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

		if(!IsFirstGame)
			timerContainer.SetActive( true );

		RemoveObjectOfHand();
		CurrentFloor.Reveal( true );

		CurrentFloor.LightsOff();

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

		if((CurrentFloor.Pickable == null && CurrentFloor.Person == null) || CurrentFloor.Deactivated)
			CurrentFloor.LightsOff();
		else
			CurrentFloor.LightsOn();
	}

	void PutObjectOnHand()
	{
		if( CurrentFloor.Pickable != null )
		{
			Sounds.PlaySound(ESoundType.PickUp);

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
		Sounds.PlaySound(ESoundType.Drop);

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
				AmountOfMatchesLeft--;

				Sounds.PlaySound(ESoundType.Interact);

				if(AmountOfMatchesLeft < MORE_BUILDINGS_TRESHOLD)
				{
					AmountOfMatchesLeft = 0;

					GenerateNewBuildings(2, 2);

					MoreBuildingsIndicator.gameObject.SetActive(true);
					moreBuildingsIndicatorShown = true;
					moreBuildingsIndicatorTimer = MORE_BUILDINGS_TIME;
				}

				if(TutorialProgress == ETutorialProgress.Interact)
				{
					UpdateTutorialState(ETutorialProgress.Timer);
				}
			}
			else
			{
				Sounds.PlaySound(ESoundType.FailInteract);
			}
		}
	}

	public void UpdateTutorialState(ETutorialProgress newProgress)
	{
		if(newProgress == ETutorialProgress.Finished)
		{
			IsFirstGame = false;
			timerContainer.SetActive(true);
		}

		HideTutorial();

		TutorialProgress = newProgress;

		if(TutorialProgress != ETutorialProgress.Finished)
			TutorialTexts[(int)TutorialProgress].gameObject.SetActive(true);
	}

	void HideTutorial()
	{
		if(TutorialProgress != ETutorialProgress.Finished)
			TutorialTexts[(int)TutorialProgress].gameObject.SetActive(false);
	}
}
