using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameStartEvent : GameEvent { }
public class OnGameEndEvent : GameEvent { }

public abstract class APlayer : MonoBehaviour {
	public enum EPlayerType
	{
		None,
		LocalPlayer,
		AIPlayer
	}

	public EPlayerType Type;
	public EPlayerColor Color;
	public Paddle Paddle;
	bool _freezePlayer = true;

	Vector3 basePosition;

	void OnEnable()
	{
		basePosition = transform.position;
		Events.Instance.AddListener<OnGameStartEvent>(HandleOnGameStart);
		Events.Instance.AddListener<OnGameEndEvent>(HandleOnGameEnd);
		Init();
	}

	void HandleOnGameEnd(OnGameEndEvent e)
	{
		Stop();
		transform.position = basePosition;
	}

	void HandleOnGameStart(OnGameStartEvent e)
	{
		Play();
	}

	public virtual void Init()
	{
		Stop();
		Paddle.PlayerColor = Color;
	}

	public virtual void Play()
	{
		_freezePlayer = false;
	}

	public virtual void Stop()
	{
		_freezePlayer = true;
	}

	protected bool CanMove()
	{
		return (_freezePlayer == false);
	}

	public void MovePaddle(float force)
	{
		if (CanMove() == true)
		{
			if (Mathf.Abs(force) > Paddle.transform.localScale.x / 6f) // If touching really close to the position, do not move
			{
				Vector3 forceVector = Vector3.zero;
				forceVector.x = 1f * (force > 0f ? 1f : -1f);
				Paddle.transform.position = Paddle.transform.position + (forceVector * Time.deltaTime * Paddle.Speed);
			}
		}
	}
}
