using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Blink : MonoBehaviour 
{
	private Image img = null;
	
	public void Start() 
	{
		img = GetComponent<Image>();
	}

	public void FixedUpdate()
	{
		Color newColor = img.color;
		newColor.a = ( Mathf.Sin(Time.time*2.0f) * 0.4f ) + 0.6f;
		img.color = newColor;
	}
}