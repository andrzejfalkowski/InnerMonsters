using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECharacterType
{
	Man,
	Woman,
	Kid,
	Teenager,
	Dog,
	Cat,
}

public enum EClothesStyle
{
	style_a,
	style_b,
	style_c
}

public class PersonOfInterest : MonoBehaviour 
{
	public ECharacterType CharacterType;
	public Thought CurrentThought;

	public SpriteRenderer FaceRenderer;
	public Sprite HappyFace;
	public Sprite SadFace;

	public SpriteRenderer BodyRenderer;
	public List<Sprite> BodySprites;

	public bool Ruined = false;
	//public EThoughtType ThoughtType;

	public SpriteRenderer ThoughtSymbolRenderer;
	public SmartTextMesh ThoughtDescription;

	public EClothesStyle ClothesStyle;

	const float THOUGHT_DESCRIPTION_TIME = 1.5f;

	// TODO: some POIs can be also contrary object for antoher POI, e.g. cat and dog can both be used against each other
	public PickableObject OwnPickableObject;

	public float ShowTime = 0f;
	bool shown = false;
	bool descriptionShown = false;

	public void Init()
	{
		Ruined = false;

		System.Array clothValues = System.Enum.GetValues (typeof(EClothesStyle));
		ClothesStyle = (EClothesStyle)clothValues.GetValue (UnityEngine.Random.Range (0, clothValues.Length));

		BodyRenderer.sprite = BodySprites[(int)ClothesStyle];

		UpdateFace();
	}

	public void Show()
	{
		shown = true;
		descriptionShown = false;

		ThoughtSymbolRenderer.gameObject.SetActive(true);
		ThoughtDescription.gameObject.SetActive(false);

		ThoughtSymbolRenderer.sprite = CurrentThought.Symbol;
	}

	public void Hide()
	{
		shown = false;
		ShowTime = 0f;
	}

	public void RevealThoughtDescription()
	{
		descriptionShown = true;

		ThoughtDescription.UnwrappedText = CurrentThought.BubbleSpeeches[UnityEngine.Random.Range (0, CurrentThought.BubbleSpeeches.Count)];
		ThoughtDescription.NeedsLayout = true;

		ThoughtSymbolRenderer.gameObject.SetActive(false);
		ThoughtDescription.gameObject.SetActive(true);
	}

	public void RevealTraumaDescription()
	{
		Ruined = true;

		descriptionShown = true;
		
		ThoughtDescription.UnwrappedText = CurrentThought.TraumaSpeeches[UnityEngine.Random.Range (0, CurrentThought.TraumaSpeeches.Count)];
		ThoughtDescription.NeedsLayout = true;

		ThoughtSymbolRenderer.gameObject.SetActive(false);
		ThoughtDescription.gameObject.SetActive(true);

		UpdateFace();
	}

	void Update()
	{
		if(shown && !descriptionShown)
		{
			ShowTime += Time.deltaTime;
			if(ShowTime >= THOUGHT_DESCRIPTION_TIME)
			{
				RevealThoughtDescription();
			}
		}
	}

	public void UpdateFace()
	{
		if(FaceRenderer != null)
			FaceRenderer.sprite = (Ruined ? HappyFace : SadFace);
	}
	
	public void UseObjectOn(PickableObject pickable)
	{
		RevealTraumaDescription ();
	}
}
