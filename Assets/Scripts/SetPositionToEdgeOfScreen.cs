using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionToEdgeOfScreen : MonoBehaviour {

	public Vector2 EdgesToStickTo = Vector2.zero;

	void OnEnable()
	{
		var screenPos = new Vector3();
		switch ((int)EdgesToStickTo.x)
		{
			case -1:
				screenPos.x = 0f;
				break;
			case 1:
				screenPos.x = Camera.main.pixelWidth;
				break;
			default:
				screenPos.x = Camera.main.pixelWidth / 2f;
				break;
		}

		switch ((int)EdgesToStickTo.y)
		{
			case -1:
				screenPos.y = 0f;
				break;
			case 1:
				screenPos.y = Camera.main.pixelHeight;
				break;
			default:
				screenPos.y = Camera.main.pixelHeight / 2f;
				break;
		}
		screenPos.z = Camera.main.transform.position.y;
		transform.position = Camera.main.ScreenToWorldPoint(screenPos);
	}
}
