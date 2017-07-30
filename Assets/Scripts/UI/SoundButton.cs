using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundButton : MonoBehaviour {

	const string SOUND_PP_KEY = "PaintPongSound";

	public AudioMixerGroup Mixer;
	public Image SoundImage;
	public Image MuteImage;
	bool sound = true;

	void OnEnable()
	{
		Load();
	}

	void Load()
	{
		sound = PlayerPrefs.GetInt(SOUND_PP_KEY, 1) == 1 ? true : false;
		Display();
	}

	void Display()
	{
		Mixer.audioMixer.SetFloat("volume", sound ? 0f : -80f);
		SoundImage.gameObject.SetActive(sound);
		MuteImage.gameObject.SetActive(sound == false);
	}

	public void TriggerButton()
	{
		sound = !sound;
		PlayerPrefs.SetInt(SOUND_PP_KEY, sound ? 1 : 0);
		Display();
		Events.Instance.Raise(new OnButtonPressedEvent());
	}

	void OnDisable()
	{
		PlayerPrefs.Save();
	}
}
