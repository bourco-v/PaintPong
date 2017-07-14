using System;
using System.Collections;
using System.Collections.Generic;
using Es.InkPainter;
using UnityEngine;

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
		ballPainter.RigidBody.AddForce(force, ForceMode);
	}

	Vector3 CalculateBounceDirection(Transform ball)
	{
		Vector3 force = Force;

		force.x = (ball.position.x - this.transform.position.x) * OrientationScale;

		return (force);
	}
}
