using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour 
{
	public Camera ParentCamera;
	public float XModifier = 1.0f;
	public float YModifier = 1.0f;

	void LateUpdate()
	{
		Vector3 newPos = this.transform.localPosition;
		newPos.x = ParentCamera.transform.localPosition.x * XModifier;
		newPos.y = ParentCamera.transform.localPosition.y * YModifier;
		this.transform.localPosition = newPos;
	}
}
