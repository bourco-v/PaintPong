using System.Collections;
using System.Collections.Generic;
using Es.InkPainter;
using UnityEngine;

public class ExplodeOnMapOnBallTouch : MonoBehaviour {

	public EPlayerColor Color;
	public Brush ExplosionBrush;
	[SerializeField]
	PaintingZone _floor;

	public void Explode()
	{
		var ball = BallPainter.Instance;
		ball.SetColor(ExplosionBrush.Color, Color);
		_floor.Paint(ExplosionBrush, BallPainter.Instance.transform.position);
	}
}
