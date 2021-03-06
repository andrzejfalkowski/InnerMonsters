﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraMgr : MonoBehaviour 
{
	public Floor currentFloor = null;

	public Button[] arrows = new Button[4];

	public GameController MyGameController;

	private float startTravelling = 0.0f;
	private float travelDistance = 0.0f;
	private Vector3 startPosition = Vector3.zero;
	private bool travelling = false;
	private Vector3 targetPosition = Vector3.zero;
	private Dir travellingDirection = Dir.NONE;


#if UNITY_ANDROID
	// faster movement as handicap for swipe delay
	private const float TRAVEL_TIME = 5.0f;
#else
	private const float TRAVEL_TIME = 3.0f;
#endif
	private const float HORIZONTAL_VELOCITY_MULTIPLIER = 2.0f;
	private const float VERTICAL_VELOCITY_MULTIPLIER = 0.8f;
	private const float Y_SHIFT = 1.0f;

	void Start()
	{
		UpdateArrows();
		currentFloor.Reveal( true );
	}

	public void GoTo( int dir )
	{
		GoTo( (Dir)dir );
	}

	public void GoTo( Dir dir )
	{
		switch( dir )
		{
			case Dir.N: 
				if(currentFloor.nextFloors[ (int)Dir.N ] == null) 
					return;
				else 
				{
					MyGameController.CurrentFloorIndex++;
					MyGameController.Sounds.PlaySound(ESoundType.SwipeUp);

					// tutorial
					if(MyGameController.TutorialProgress == ETutorialProgress.ArrowKeys 
				  		&& MyGameController.CurrentFloorIndex == 2)
						MyGameController.UpdateTutorialState(ETutorialProgress.PickUp);
				}
				break;
			case Dir.E: 
				if(currentFloor.nextFloors[ (int)Dir.E ] == null) 
					return;
				else
				{
					MyGameController.CurrentBuildingIndex++;
					MyGameController.Sounds.PlaySound(ESoundType.SwipeSide);

					// tutorial
					if(MyGameController.TutorialProgress == ETutorialProgress.Timer 
					   && MyGameController.CurrentBuildingIndex == 1)
						MyGameController.UpdateTutorialState(ETutorialProgress.Finished);
				}
				break;
			case Dir.S: 
				if(currentFloor.nextFloors[ (int)Dir.S ] == null) 
					return;
				else 
				{
					MyGameController.CurrentFloorIndex--;
					MyGameController.Sounds.PlaySound(ESoundType.SwipeDown);

					// tutorial
					if(MyGameController.TutorialProgress == ETutorialProgress.PickUp 
					   && MyGameController.CurrentFloorIndex == 0
				   		&& MyGameController.CurrentlyPickedUpObject != null)
						MyGameController.UpdateTutorialState(ETutorialProgress.Interact);
				}
				break;
			case Dir.W: 
				if(currentFloor.nextFloors[ (int)Dir.W ] == null) 
					return;
				else 
				{
					MyGameController.CurrentBuildingIndex--;
					MyGameController.Sounds.PlaySound(ESoundType.SwipeSide);

					// tutorial
					if(MyGameController.TutorialProgress == ETutorialProgress.Timer 
					   && MyGameController.CurrentBuildingIndex == -1)
						MyGameController.UpdateTutorialState(ETutorialProgress.Finished);
				}
				break;
		}
		
		startTravelling = Time.time;
		travelDistance = currentFloor.GetDistanceToFloor( dir );
		startPosition = transform.position;

		currentFloor.Reveal( false );
		currentFloor = currentFloor.nextFloors[ (int)dir ];

		if( dir == Dir.N || dir == Dir.S )
			currentFloor.Reveal( true );

		travelling = true;

		targetPosition = new Vector3(currentFloor.transform.position.x, currentFloor.transform.position.y + Y_SHIFT, startPosition.z);
		travellingDirection = dir;
	}

	public void Snap( )
	{
		travelling = false;
		transform.position = new Vector3(currentFloor.transform.position.x, currentFloor.transform.position.y + Y_SHIFT, transform.position.z);
	}

	public void UpdateArrows()
	{
		arrows[ (int)Dir.N ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.N ] != null );
		arrows[ (int)Dir.E ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.E ] != null );
		arrows[ (int)Dir.S ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.S ] != null );
		arrows[ (int)Dir.W ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.W ] != null );
		currentFloor.Reveal( true );
	}

	// Update is called once per frame
	void Update () 
	{
		if( travelling )
		{
			float distanceCovered = (Time.time - startTravelling) * TRAVEL_TIME * travelDistance;

			if( travellingDirection == Dir.N || travellingDirection == Dir.S )
				distanceCovered *= HORIZONTAL_VELOCITY_MULTIPLIER;
			else
				distanceCovered *= VERTICAL_VELOCITY_MULTIPLIER;

			float percentageCovered = distanceCovered / travelDistance;

			if( percentageCovered > 1.0f )
			{
				if( travellingDirection == Dir.E || travellingDirection == Dir.W )
					currentFloor.Reveal( true );

				transform.position = targetPosition;
				travelling = false;
				travellingDirection = Dir.NONE;
				UpdateArrows();
			}
			else
			{
				transform.position = Vector3.Lerp( startPosition, targetPosition, percentageCovered );
			}
		}
		else
		{
#if UNITY_ANDROID
			if(MyGameController.GetCurrentTapType() == EAndroidTapType.Right)
				GoTo (Dir.E);
			else if(MyGameController.GetCurrentTapType() == EAndroidTapType.Left)
				GoTo (Dir.W);
			else if(MyGameController.GetCurrentTapType() == EAndroidTapType.Up)
				GoTo (Dir.N);
			else if(MyGameController.GetCurrentTapType() == EAndroidTapType.Down)
				GoTo (Dir.S);
#else
			if (Input.GetKey ("right"))
				GoTo (Dir.E);
			else if (Input.GetKey ("left"))
				GoTo (Dir.W);
			else if (Input.GetKey ("up"))
				GoTo (Dir.N);
			else if (Input.GetKey ("down"))
				GoTo (Dir.S);
#endif
		}
	}
}
