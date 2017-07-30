using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : UIElement {

	public PaintingZone PaintingZone;
	public List<Text> BluePercent = new List<Text>();
	public List<Text> RedPercent = new List<Text>();
	public List<Text> Winner = new List<Text>();

	void OnEnable()
	{
		Hide();
		Events.Instance.AddListener<OnGameEndEvent>(HandleOnGameEnd);
		Events.Instance.AddListener<OnStateChangedEvent>(HandleOnStateChanged);
	}

	void HandleOnStateChanged(OnStateChangedEvent e)
	{
		if (e.State == EGameState.None)
			Hide(0.5f);
	}

	void HandleOnGameEnd(OnGameEndEvent e)
	{
		for (int i = 0; i < BluePercent.Count; ++i)
		{
			BluePercent[i].text = PaintingZone.GetPlayerScorePercent(EPlayerColor.BluePlayer).ToString("0.#") + "%"; 
		}
		for (int i = 0; i < RedPercent.Count; ++i)
		{
			RedPercent[i].text = PaintingZone.GetPlayerScorePercent(EPlayerColor.RedPlayer).ToString("0.#") + "%";
		}
		for (int i = 0; i < Winner.Count; ++i)
		{
			Winner[i].text = PaintingZone.GetWinner() == EPlayerColor.BluePlayer ? "Blue Wins!" : "Red Wins!";
		}
		Show(0.5f);
	}
}
