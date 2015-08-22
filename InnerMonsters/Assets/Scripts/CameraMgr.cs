using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Assertions;
using System.Collections;

public class CameraMgr : MonoBehaviour 
{
	public Floor currentFloor = null;

	public Button[] arrows = new Button[4];

	private float startTravelling = 0.0f;
	private float travelDistance = 0.0f;
	private Vector3 startPosition = Vector3.zero;
	private bool travelling = false;
	private Vector3 targetPosition = Vector3.zero;

	private const float TRAVEL_TIME = 3.0f;

	void Start()
	{
		//Assert.IsNotNull<Floor>( currentFloor );
		UpdateArrows();
	}

	public void GoTo( int dir )
	{
		GoTo( (Dir)dir );
	}

	public void GoTo( Dir dir )
	{
		switch( dir )
		{
			case Dir.N: if(currentFloor.nextFloors[ (int)Dir.N ] == null) return; break;
			case Dir.E: if(currentFloor.nextFloors[ (int)Dir.E ] == null) return; break;
			case Dir.S: if(currentFloor.nextFloors[ (int)Dir.S ] == null) return; break;
			case Dir.W: if(currentFloor.nextFloors[ (int)Dir.W ] == null) return; break;
		}
		
		startTravelling = Time.time;
		travelDistance = currentFloor.GetDistanceToFloor( dir );
		startPosition = transform.position;
		currentFloor = currentFloor.nextFloors[ (int)dir ];
		travelling = true;

		targetPosition = new Vector3(currentFloor.transform.position.x, currentFloor.transform.position.y, startPosition.z);
	}

	void UpdateArrows()
	{
		arrows[ (int)Dir.N ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.N ] != null );
		arrows[ (int)Dir.E ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.E ] != null );
		arrows[ (int)Dir.S ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.S ] != null );
		arrows[ (int)Dir.W ].gameObject.SetActive( currentFloor.nextFloors[ (int)Dir.W ] != null );
	}

	// Update is called once per frame
	void Update () 
	{
		if( travelling )
		{
			float distanceCovered = (Time.time - startTravelling) * TRAVEL_TIME * travelDistance;
			float percentageCovered = distanceCovered / travelDistance;

			if( percentageCovered > 1.0f )
			{
				transform.position = targetPosition;
				travelling = false;
				UpdateArrows();
			}
			else
			{
				transform.position = Vector3.Lerp( startPosition, targetPosition, percentageCovered );
			}
		}
	}
}
