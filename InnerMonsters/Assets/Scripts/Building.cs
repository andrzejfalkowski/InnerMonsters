using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour 
{
	public List<Floor> Floors;
	public int Height
	{
		get { return Floors.Count; }
	}
}
