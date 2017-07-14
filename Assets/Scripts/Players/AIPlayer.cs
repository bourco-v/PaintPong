using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : APlayer
{

	public float AngleOfAim = 1.2f; // Distance from the center of the paddle the AI will try to touch the ball
	public float ConfidenceSpeed = 1f; // Speed of the AI after it touches the ball. Reduce chances of suicide
	public float EnemyShotSpeed = 0.6f; // Speed of the AI when blocking an enemy shot. Reduce chances of blocking enemy shot
	float _choosenAngle = 0f;

	public override void Init()
	{
		Type = EPlayerType.AIPlayer;
		ConfidenceSpeed *= Paddle.Speed; // Base values are only percentage of speed based on Paddle Speed;
		EnemyShotSpeed *= Paddle.Speed;
		base.Init();
	}

	void Update()
	{
		if (CanMove())
		{
			Paddle.Speed = ((BallPainter.Instance.CurrentPlayerColor == Color) ? ConfidenceSpeed : EnemyShotSpeed);
			var xDistance = BallPainter.Instance.transform.position.x - Paddle.transform.position.x;
			if (Mathf.Abs(BallPainter.Instance.transform.position.z - Paddle.transform.position.z) > 2f ||
				Mathf.Abs(xDistance) > AngleOfAim) // If far form the ball, only try to block it
			{
				MovePaddle(xDistance);
			}
			else // If out of danger, aim a shot
			{
				if (_choosenAngle == 0f || (BallPainter.Instance.CurrentPlayerColor == EPlayerColor.RedPlayer))
					_choosenAngle = Random.Range(-AngleOfAim, AngleOfAim);
				MovePaddle(xDistance + _choosenAngle);
			}
			if (BallPainter.Instance.CurrentPlayerColor == EPlayerColor.RedPlayer)
				_choosenAngle = 0f;
		}
	}
}
