using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EFacadeType
{
	blue,
	red
}

public enum EFrameType
{
	frame_a,
	frame_b
}

public class Building : MonoBehaviour 
{
	public EFacadeType FacadeType;
	public EFrameType FrameType;

	public List<Floor> Floors;
	// in case of basements, building starts underground
	public int BaseLevel = 0;

	public int Height
	{
		get { return Floors.Count; }
	}

	public int HeightOverGround
	{
		get { return Floors.Count + BaseLevel; }
	}
	
	public void Init()
	{
		System.Array facadeValues = System.Enum.GetValues (typeof(EFacadeType));
		FacadeType = (EFacadeType)facadeValues.GetValue (UnityEngine.Random.Range (0, facadeValues.Length));

		System.Array frameValues = System.Enum.GetValues (typeof(EFrameType));
		FrameType = (EFrameType)facadeValues.GetValue (UnityEngine.Random.Range (0, frameValues.Length));
	}

	public Floor GetGroundFloor()
	{
		return Floors[0 - BaseLevel];
	}

}
