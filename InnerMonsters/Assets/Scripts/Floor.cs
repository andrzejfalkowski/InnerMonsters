using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EBackgroundType
{
	floor_1,
	floor_2,
	floor_3,
	floor_4,
	floor_5,
	floor_6,
}

public class Floor : MonoBehaviour 
{
	public EBackgroundType BackgroundType;

	public SpriteRenderer BackgroundSpriteRenderer;
	public SpriteRenderer ForegroundSpriteRenderer;

	public PersonOfInterest Person;
	public PickableObject Pickable;

	public List<Sprite> BackgroundSprites;

	public List<Sprite> ForegroundSprites;
	public List<Sprite> ForegroundBottomSprites;
	public List<Sprite> ForegroundTopSprites;
	public List<Sprite> ForegroundBasementSprites;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(EFacadeType facadeType, bool isTopFloor, bool isBottomFloor, bool isBasement)
	{
		System.Array backgroundValues = System.Enum.GetValues(typeof(EBackgroundType));
		BackgroundType = (EBackgroundType)backgroundValues.GetValue(UnityEngine.Random.Range(0, backgroundValues.Length));

		if((int)BackgroundType < BackgroundSprites.Count)
			BackgroundSpriteRenderer.sprite = BackgroundSprites[(int)BackgroundType];

		if(isTopFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundTopSprites[(int)facadeType];
		}
		else if(isBottomFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundBottomSprites[(int)facadeType];
		}
		else if(isBasement)
		{
			ForegroundSpriteRenderer.sprite = ForegroundBasementSprites[(int)facadeType];
		}
		else 
		{
			ForegroundSpriteRenderer.sprite = ForegroundSprites[(int)facadeType];
		}
	}
}
