using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
	public List<Building> Buildings;
	public int Width
	{
		get { return Buildings.Count; }
	}
}
