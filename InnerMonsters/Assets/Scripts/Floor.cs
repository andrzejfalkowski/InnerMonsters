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
enum Fade { NONE, IN, OUT }

public class Floor : MonoBehaviour 
{
	public EBackgroundType BackgroundType;

	public SpriteRenderer BackgroundSpriteRenderer;
	public SpriteRenderer BackgroundFrameSpriteRenderer;
	public SpriteRenderer ForegroundSpriteRenderer;

	public PersonOfInterest Person;
	public PickableObject Pickable;
	public Floor[] nextFloors = new Floor[4];

	private Fade fade = Fade.NONE;
	private const float FADE_SPEED = 0.05f;

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

	// Use this for initialization
//	void Start () {
//	
//	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}

	public void Init(EFacadeType facadeType, EFrameType frameType, bool isTopFloor, bool isBottomFloor, bool isBasement)
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
	}

	public void Reveal( bool reveal )
	{
		fade = ( reveal ? Fade.OUT : Fade.IN );
	}

	void StopFade( float alpha )
	{
		Color newColor = ForegroundSpriteRenderer.color;
		newColor.a = alpha;
		ForegroundSpriteRenderer.color = newColor;
		fade = Fade.NONE;
	}

	void FixedUpdate()
	{
		switch( fade )
		{
			case Fade.IN:
			{
				Color newColor = ForegroundSpriteRenderer.color;
				newColor.a += FADE_SPEED;

				if( newColor.a < 1.0f ) 
					ForegroundSpriteRenderer.color = newColor;
				else
					StopFade( 1.0f );

				break;
			}

			case Fade.OUT:
			{
				Color newColor = ForegroundSpriteRenderer.color;
				newColor.a -= FADE_SPEED * 2.0f;	// Fade out at double velocity than Fade in

				if( newColor.a > 0.0f ) 
					ForegroundSpriteRenderer.color = newColor;
				else
					StopFade( 0.0f );

				break;
			}
		}
	}
}
