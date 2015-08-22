using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EFacadeType
{
	blue,
	red
}

public class Building : MonoBehaviour 
{
	public EFacadeType FacadeType;

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
	}

	public Floor GetGroundFloor()
	{
		return Floors[0 - BaseLevel];
	}

}
