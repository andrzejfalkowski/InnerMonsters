using UnityEngine;
using System.Collections;
using System;

public class TransitionController : MonoBehaviour 
{
	public UnityEngine.UI.Image BlackImage;
	private Fade fade = Fade.NONE;

	const float TRANSITION_TIME = 0.25f;

	private Action afterFadeInCallback;
	private Action afterFadeOutCallback;
	public void StartFade(Action inCallback, Action outCallback)
	{
		afterFadeInCallback = inCallback;
		afterFadeOutCallback = outCallback;
		fade = Fade.IN;
	}

	void FixedUpdate()
	{
		switch( fade )
		{
			case Fade.IN:
			{
				float newAlpha = BlackImage.color.a + Time.fixedDeltaTime / TRANSITION_TIME;
				
				if( newAlpha < 1.0f ) 
					UpdateForegroundAlpha( newAlpha );
				else
				{
					StopFade( 1.0f, Fade.OUT );

					if(afterFadeInCallback != null)
					{
						afterFadeInCallback();
						afterFadeInCallback = null;
					}
				}
				
				break;
			}
				
			case Fade.OUT:
			{
				float newAlpha = BlackImage.color.a - Time.fixedDeltaTime / TRANSITION_TIME;
				
				if( newAlpha > 0.0f ) 
					UpdateForegroundAlpha( newAlpha );
				else
				{
					StopFade( 0.0f, Fade.NONE );

					if(afterFadeOutCallback != null)
					{
						afterFadeOutCallback();
						afterFadeOutCallback = null;
					}
				}
	
				break;
			}
		}
	}

	void StopFade( float alpha, Fade nextFadeState)
	{
		UpdateForegroundAlpha( alpha );
		fade = nextFadeState;
	}
	
	void UpdateForegroundAlpha( float alpha )
	{
		Color newColor = BlackImage.color;
		newColor.a = alpha;
		
		BlackImage.color = newColor;
	}
}
