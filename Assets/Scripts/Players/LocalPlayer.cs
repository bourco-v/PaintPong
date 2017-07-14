using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : APlayer {

	const float MAX_Z_DISTANCE_BETWEEN_TOUCH_AND_PADDLE = 4f;

	public override void Init()
	{
		Type = EPlayerType.LocalPlayer;
		base.Init();
	}

	void Update()
	{
		for (int i = 0; i < Input.touchCount; ++i)
		{
			var touchPosition = Input.GetTouch(i).position;
			TouchedScreen(touchPosition);
		}
		if (Input.GetMouseButton(0)) // Use also mouse to move paddles
		{
			TouchedScreen(Input.mousePosition);
		}
	}

	void TouchedScreen(Vector2 screenPos)
	{
		var worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
		if (TouchedNearPaddle(worldPosition) == true)
		{
			MovePaddle(worldPosition.x - Paddle.transform.position.x);
		}
	}

	bool TouchedNearPaddle(Vector3 worldPos) // If the touch was near the paddle, consider it your touch and not any other localplayer touch
	{
		return (System.Math.Abs(worldPos.z - Paddle.transform.position.z) < MAX_Z_DISTANCE_BETWEEN_TOUCH_AND_PADDLE);
	}
}
