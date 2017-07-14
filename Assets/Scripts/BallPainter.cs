using System;
using System.Collections;
using System.Collections.Generic;
using Es.InkPainter;
using Es.InkPainter.Sample;
using UnityEngine;

public class BallPainter : CollisionPainter {

	public static BallPainter Instance
	{
		get; private set;
	}

	public Rigidbody RigidBody { get; protected set; }
	public EPlayerColor CurrentPlayerColor { get; protected set; } // Defines which player touched the ball last

	Vector3 basePos;

	void OnEnable()
	{
		basePos = transform.position;
		Events.Instance.AddListener<OnGameStartEvent>(HandleOnGameStart);
		Events.Instance.AddListener<OnGameEndEvent>(HandleOnGameEnd);
		if (Instance != null)
			DestroyObject(gameObject);
		else
		{
			Instance = this;
			RigidBody = GetComponent<Rigidbody>();
			SetColor(Color.white, EPlayerColor.None);
		}
	}

	void HandleOnGameEnd(OnGameEndEvent e)
	{
		transform.position = basePos;
	}

	void HandleOnGameStart(OnGameStartEvent e)
	{
		CurrentPlayerColor = EPlayerColor.None;
	}

	public void SetColor(Color color, EPlayerColor playerColor)
	{
		CurrentPlayerColor = playerColor;
		brush.Color = color;
		SetColor();
	}

	protected override void OnCollisionStay(Collision collision)
	{
		if (IsPaintCollisionReady())
		{
			for (int i = 0; i < collision.contacts.Length; ++i)
			{
				var canvas = collision.contacts[i].otherCollider.GetComponent<InkCanvas>();
				if (canvas != null)
				{
					canvas.Paint(brush, collision.contacts[i].point);
				}
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		for (int i = 0; i < collision.contacts.Length; ++i)
		{
			var paddle = collision.contacts[i].otherCollider.GetComponent<Paddle>();
			var explosionWall = collision.contacts[i].otherCollider.GetComponent<ExplodeOnMapOnBallTouch>();
			if (paddle != null)
			{
				paddle.OnCollisionWithBall(this);
				SetColor();
			}
			if (explosionWall != null)
			{
				explosionWall.Explode();
			}
		}
	}
}
