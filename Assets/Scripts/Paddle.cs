using System;
using System.Collections;
using System.Collections.Generic;
using Es.InkPainter;
using UnityEngine;

public class OnCollisionWithPaddleEvent : GameEvent { }

public class Paddle : MonoBehaviour {

	public EPlayerColor PlayerColor;
	public Brush Brush = null;
	public Vector3 Force;
	public ForceMode ForceMode;
	public float OrientationScale = 50f;
	public float Speed;

	public void OnCollisionWithBall(BallPainter ballPainter)
	{
		ballPainter.SetColor(Brush.Color, PlayerColor);
		var force = CalculateBounceDirection(ballPainter.transform);
		ballPainter.RigidBody.velocity = Vector3.zero;
		ballPainter.RigidBody.AddForce(force, ForceMode);
		Events.Instance.Raise(new OnBallBounceEvent());
		Events.Instance.Raise(new OnCollisionWithPaddleEvent());
	}

	Vector3 CalculateBounceDirection(Transform ball)
	{
		Vector3 force = Force;

		force.x = Mathf.Clamp(((ball.position.x - this.transform.position.x) * OrientationScale), -8f, 8f); // Clamp to avoid too slow shots covering the whole area

		return (force);
	}
}
