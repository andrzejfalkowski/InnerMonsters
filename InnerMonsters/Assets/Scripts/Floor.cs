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

public enum Dir { N, E, S, W, NONE }

public class Floor : MonoBehaviour 
{
	public EBackgroundType BackgroundType;

	public SpriteRenderer BackgroundSpriteRenderer;
	public SpriteRenderer BackgroundFrameSpriteRenderer;
	public SpriteRenderer ForegroundSpriteRenderer;
	public SpriteRenderer ForegroundPatternSpriteRenderer;

	public PersonOfInterest Person;
	public PickableObject Pickable;
	public Floor[] nextFloors = new Floor[4];

	private const float FADE_SPEED = 0.2f;

	public float GetDistanceToFloor( Dir dir )
	{
		switch( dir )
		{
			case Dir.N: return nextFloors[ (int)Dir.N ].transform.position.y - transform.position.y; break;
			case Dir.E: return nextFloors[ (int)Dir.E ].transform.position.x - transform.position.x; break;
			case Dir.S: return transform.position.y - nextFloors[ (int)Dir.S ].transform.position.y; break;
			case Dir.W: return transform.position.x - nextFloors[ (int)Dir.W ].transform.position.x; break;
		}
		return 0.0f;
	}

	public List<Sprite> BackgroundSprites;
	public List<Sprite> BackgroundFrameSprites;

	public List<Sprite> ForegroundSprites;
	public List<Sprite> ForegroundBottomSprites;
	public List<Sprite> ForegroundTopSprites;
	public List<Sprite> ForegroundBasementSprites;
	public List<Sprite> ForegroundPatternSprites;

	// Use this for initialization
//	void Start () {
//	
//	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}

	public void Init(EFacadeType facadeType, EFrameType frameType, Color patternColor, bool isTopFloor, bool isBottomFloor, bool isBasement)
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

		BackgroundFrameSpriteRenderer.sprite = BackgroundFrameSprites [(int)frameType];

		if (isBasement)
		{
			ForegroundPatternSpriteRenderer.sprite = null;
		}
		else
		{
			ForegroundPatternSpriteRenderer.sprite = ForegroundPatternSprites[UnityEngine.Random.Range (0, ForegroundPatternSprites.Count)];
			ForegroundPatternSpriteRenderer.color = patternColor;
		}
	}

	public void Reveal( bool reveal )
	{
		StartCoroutine( reveal ? FadeOut() : FadeIn() );
	}

	IEnumerator FadeIn() 
	{
		while( ForegroundSpriteRenderer.color.a < 1.0f ) 
		{
			Color newColor = ForegroundSpriteRenderer.color;
			newColor.a += FADE_SPEED;
			ForegroundSpriteRenderer.color = newColor;

			newColor = ForegroundPatternSpriteRenderer.color;
			newColor.a += FADE_SPEED;
			ForegroundPatternSpriteRenderer.color = newColor;

			yield return new WaitForSeconds( 0.1f ); // Random.Range(5, 10) );
		}
	}

	IEnumerator FadeOut() 
	{
		while( ForegroundSpriteRenderer.color.a > 0.0f ) 
		{
			Color newColor = ForegroundSpriteRenderer.color;
			newColor.a -= FADE_SPEED * 2.0f;
			ForegroundSpriteRenderer.color = newColor;

			newColor = ForegroundPatternSpriteRenderer.color;
			newColor.a -= FADE_SPEED * 2.0f;
			ForegroundPatternSpriteRenderer.color = newColor;

			yield return new WaitForSeconds( 0.1f ); // Random.Range(5, 10) );
		}
	}
}
