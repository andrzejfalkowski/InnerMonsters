using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roof : MonoBehaviour 
{
	public SpriteRenderer RoofSpriteRenderer;
	public List<Sprite> RoofSprites;

	public void Init()
	{
		RoofSpriteRenderer.sprite = RoofSprites[UnityEngine.Random.Range(0, RoofSprites.Count)];
	}
}