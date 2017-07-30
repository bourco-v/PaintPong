using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnStateChangedEvent : GameEvent {
	public EGameState State;
}
public class OnTimerEvent : GameEvent { }

public enum EGameState
{
	None,
	BeforeGame,
	Game,
	AfterGame,
}

public class GameTimer : MonoBehaviour {

	const float BEFORE_GAME_TIMER = 2f;
	const float GAME_TIMER = 30f;
	const float AFTER_GAME_TIMER = 6f;

	public List<Text> Timers = new List<Text>();
	float _timer;
	float _countDownTimer = 1f;
	public EGameState State = EGameState.None;
	

	void Update()
	{
		switch (State)
		{
			case EGameState.BeforeGame:
				_countDownTimer -= Time.deltaTime;
				if (_countDownTimer <= 0f)
				{
					Events.Instance.Raise(new OnTimerEvent());
					_countDownTimer = 1f;
				}
				_timer -= Time.deltaTime;
				if (_timer < 0f)
					NextState();
				break;
			case EGameState.Game:
			case EGameState.AfterGame:
				_countDownTimer -= Time.deltaTime;
				_timer -= Time.deltaTime;
				if (_timer < 0f)
					NextState();
				break;
			default:
				break;
		}
		ShowTimer();
	}

	public void StartGame()
	{
		NextState();
	}

	void NextState()
	{
		switch (State)
		{
			case EGameState.BeforeGame:
				State = EGameState.Game;
				_timer = GAME_TIMER;
				Events.Instance.Raise(new OnTimerEvent());
				Events.Instance.Raise(new OnGameStartEvent());
				break;
			case EGameState.Game:
				State = EGameState.AfterGame;
				_timer = AFTER_GAME_TIMER;
				Events.Instance.Raise(new OnGameEndEvent());
				break;
			case EGameState.AfterGame:
				State = EGameState.None;
				_timer = 0f;
				break;
			case EGameState.None:
				State = EGameState.BeforeGame;
				_timer = BEFORE_GAME_TIMER;
				_countDownTimer = 1f;
				Events.Instance.Raise(new OnTimerEvent());
				break;
		}
		ShowTimer();
		Events.Instance.Raise(new OnStateChangedEvent() { State = State });
	}

	void ShowTimer()
	{

		for (int i = 0; i < Timers.Count; ++i)
		{
			if (State == EGameState.BeforeGame || State == EGameState.Game)
			{
				Timers[i].text = String.Format("{0:00}", _timer);
			}
			else
			{
				Timers[i].text = string.Empty;
			}
		}
	}
}
