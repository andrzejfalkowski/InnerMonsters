using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour 
{
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

}
