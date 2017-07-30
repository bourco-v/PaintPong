using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : APlayer
{
	public enum EStrategy
	{
		Hit,
		Miss
	}

	public float AngleOfAim = 1.2f; // Distance from the center of the paddle the AI will try to touch the ball
	public float HitChances = 0.85f;
	float _choosenAngle = 0f;
	EStrategy _currentStrategy = EStrategy.Hit;

	public override void Init()
	{
		Type = EPlayerType.AIPlayer;
		base.Init();
		Events.Instance.AddListener<OnBallBounceEvent>(HandleOnBallBounce);
	}

	void OnDisable()
	{
		Events.Instance.RemoveListener<OnBallBounceEvent>(HandleOnBallBounce);
	}

	private void HandleOnBallBounce(OnBallBounceEvent e)
	{
		_choosenAngle = UnityEngine.Random.Range(-AngleOfAim, AngleOfAim);
		if (UnityEngine.Random.Range(0f, 1f) <= HitChances) // AI Will touch the ball if hitchance
		{
			_currentStrategy = EStrategy.Hit;
		}
		else
		{
			_currentStrategy = EStrategy.Miss;
			_choosenAngle += _choosenAngle > 0f ? AngleOfAim * 4f : -AngleOfAim * 4f;
		}
	}

	void Update()
	{
		if (CanMove())
		{
			switch (_currentStrategy)
			{
				case EStrategy.Miss:
					{
						// Do not move. Can still stop balls but less likely
					}
					break;
				case EStrategy.Hit:
					{
						if (Mathf.Abs(BallPainter.Instance.transform.position.z - Paddle.transform.position.z) <= 5f) // Follow the ball when near
						{
							var xDistance = BallPainter.Instance.transform.position.x - Paddle.transform.position.x;
							var position = Paddle.transform.position;
							position.x += xDistance + _choosenAngle;
							MovePaddle(position);
						}
						else // Go back to center
						{
							var pos = basePosition;
							pos.z = Paddle.transform.position.z;
							pos.y = Paddle.transform.position.y;
							MovePaddle(pos);
						}
					}
					break;
			}
		}
	}
}
