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
enum Fade { NONE, IN, OUT, IN_SPECIAL }

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

	private Fade fade = Fade.NONE;
	private const float FADE_SPEED = 0.05f;

	public bool Deactivated = false;

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

	bool isTopFloor;
	bool isBottomFloor;
	bool isBasement;
	// Use this for initialization
//	void Start () {
//	
//	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}

	public void Init(EPatternColor patternColor, bool _isTopFloor, bool _isBottomFloor, bool _isBasement)
	{
		isTopFloor = _isTopFloor;
		isBottomFloor = _isBottomFloor;
		isBasement = _isBasement;

		System.Array backgroundValues = System.Enum.GetValues(typeof(EBackgroundType));
		BackgroundType = (EBackgroundType)backgroundValues.GetValue(UnityEngine.Random.Range(0, backgroundValues.Length));

		if((int)BackgroundType < BackgroundSprites.Count)
			BackgroundSpriteRenderer.sprite = BackgroundSprites[(int)BackgroundType];

		if(isTopFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundTopSprites[0];
		}
		else if(isBottomFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundBottomSprites[0];
		}
		else if(isBasement)
		{
			ForegroundSpriteRenderer.sprite = ForegroundBasementSprites[0];
		}
		else 
		{
			ForegroundSpriteRenderer.sprite = ForegroundSprites[0];
		}

		BackgroundFrameSpriteRenderer.sprite = BackgroundFrameSprites [0];

		if (isBasement)
		{
			ForegroundPatternSpriteRenderer.sprite = null;
		}
		else
		{
			int variant = UnityEngine.Random.Range(0, 2);
			ForegroundPatternSpriteRenderer.sprite = ForegroundPatternSprites[((int)patternColor * 3) + variant];
		}
	}

	public void Reveal( bool reveal, bool special = false )
	{
		if (Deactivated && reveal)
			return;

		fade = ( special ? Fade.IN_SPECIAL : ( reveal ? Fade.OUT : Fade.IN ) );

		if(Person != null)
		{
			if(reveal)
				Person.Show();
			else
				Person.Hide();
		}
	}

	void StopFade( float alpha )
	{
		UpdateForegroundAlpha( alpha );
		fade = Fade.NONE;
	}

	void UpdateForegroundAlpha( float alpha )
	{
		Color newColor = ForegroundSpriteRenderer.color;
		newColor.a = alpha;

		ForegroundSpriteRenderer.color = newColor;

		newColor = ForegroundPatternSpriteRenderer.color;
		newColor.a = alpha;

		ForegroundPatternSpriteRenderer.color = newColor;
	}

	public void Deactivate()
	{
		
		Reveal(false, true);

		Deactivated = true;

		if(isTopFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundTopSprites[1];
		}
		else if(isBottomFloor)
		{
			ForegroundSpriteRenderer.sprite = ForegroundBottomSprites[1];
		}
		else if(!isBasement)
		{
			ForegroundSpriteRenderer.sprite = ForegroundSprites[1];
		}
	}

	void FixedUpdate()
	{
		switch( fade )
		{
			case Fade.IN:
			{
				float newAlpha = ForegroundSpriteRenderer.color.a + FADE_SPEED;

				if( newAlpha < 1.0f ) 
					UpdateForegroundAlpha( newAlpha );
				else
					StopFade( 1.0f );

				break;
			}

			case Fade.OUT:
			{
				// Fade out at double velocity than Fade in
				float newAlpha = ForegroundSpriteRenderer.color.a - ( FADE_SPEED * 2.0f );

				if( newAlpha > 0.0f ) 
					UpdateForegroundAlpha( newAlpha );
				else
					StopFade( 0.0f );

				break;
			}

			case Fade.IN_SPECIAL:
			{
				float newAlpha = ForegroundSpriteRenderer.color.a + FADE_SPEED / 5.0f;
				
				if( newAlpha < 1.0f ) 
					UpdateForegroundAlpha( newAlpha );
				else
					StopFade( 1.0f );
				
				break;
			}
		}
	}
}
