using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OnButtonPressedEvent : GameEvent { }

public class SoundManager : MonoBehaviour {

	public AudioSource lowEffects;
	public AudioSource medEffects;
	public AudioSource highEffects;
	public AudioMixerGroup Mixer;

	public AudioClip ExplosionSound;
	public AudioClip WallCollisionSound;
	public AudioClip PlayerCollisionSound;
	public AudioClip EndSound;
	public AudioClip BeginSound;
	public AudioClip ButtonSound;
	public AudioClip TimerSound;

	void OnEnable()
	{
		lowEffects.outputAudioMixerGroup = Mixer;
		medEffects.outputAudioMixerGroup = Mixer;
		highEffects.outputAudioMixerGroup = Mixer;
		Events.Instance.AddListener<OnGameStartEvent>(HandleOnGameStart);
		Events.Instance.AddListener<OnGameEndEvent>(HandleOnGameEnd);
		Events.Instance.AddListener<OnTimerEvent>(HandleOnTimer);
		Events.Instance.AddListener<OnExplosionEvent>(HandleOnExplosion);
		Events.Instance.AddListener<OnCollisionWithPaddleEvent>(HandleOnCollisionWithPaddle);
		Events.Instance.AddListener<OnCollisionWithWallEvent>(HandleOnCollisionWithWall);
		Events.Instance.AddListener<OnButtonPressedEvent>(HandleOnButtonPressed);
	}

	void OnDisable()
	{
		Events.Instance.RemoveListener<OnGameStartEvent>(HandleOnGameStart);
		Events.Instance.RemoveListener<OnGameEndEvent>(HandleOnGameEnd);
		Events.Instance.RemoveListener<OnTimerEvent>(HandleOnTimer);
		Events.Instance.RemoveListener<OnExplosionEvent>(HandleOnExplosion);
		Events.Instance.RemoveListener<OnCollisionWithPaddleEvent>(HandleOnCollisionWithPaddle);
		Events.Instance.RemoveListener<OnCollisionWithWallEvent>(HandleOnCollisionWithWall);
		Events.Instance.RemoveListener<OnButtonPressedEvent>(HandleOnButtonPressed);
	}

	private void HandleOnButtonPressed(OnButtonPressedEvent e)
	{
		PlayEffect(ButtonSound);
	}

	private void HandleOnCollisionWithWall(OnCollisionWithWallEvent e)
	{
		PlayEffect(WallCollisionSound);
	}

	private void HandleOnCollisionWithPaddle(OnCollisionWithPaddleEvent e)
	{
		PlayEffect(PlayerCollisionSound, 1);
	}

	private void HandleOnExplosion(OnExplosionEvent e)
	{
		PlayEffect(ExplosionSound, 2);
	}

	private void HandleOnTimer(OnTimerEvent e)
	{
		Debug.Log("Timer");
		PlayEffect(TimerSound, 1);
	}

	private void HandleOnGameEnd(OnGameEndEvent e)
	{
		PlayEffect(EndSound, 2);
	}

	private void HandleOnGameStart(OnGameStartEvent e)
	{
		PlayEffect(BeginSound, 2);
	}

	public void PlayEffect(AudioClip effect, int order = 0)
	{
		AudioSource source = lowEffects;
		if (order == 1)
			source = medEffects;
		if (order == 2)
			source = highEffects;
		source.clip = effect;
		source.Play();
	}
}
