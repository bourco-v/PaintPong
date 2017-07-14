using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeMenu : UIElement {

	public GameObject SpectatorGameMode;
	public GameObject SoloGameMode;
	public GameObject DuoGameMode;

	void OnEnable()
	{
		Events.Instance.AddListener<OnStateChangedEvent>(HandleOnStateChanged);
	}

	void HandleOnStateChanged(OnStateChangedEvent e)
	{
		if (e.State == EGameState.None)
		{
			Show(0.5f);
		}
	}

	public void SpectatorButton()
	{
		SpectatorGameMode.SetActive(true);
		SoloGameMode.SetActive(false);
		DuoGameMode.SetActive(false);
		Hide(0.5f);
	}

	public void SoloButton()
	{
		SpectatorGameMode.SetActive(false);
		SoloGameMode.SetActive(true);
		DuoGameMode.SetActive(false);
		Hide(0.5f);
	}

	public void DuoButton()
	{
		SpectatorGameMode.SetActive(false);
		SoloGameMode.SetActive(false);
		DuoGameMode.SetActive(true);
		Hide(0.5f);
	}
}
