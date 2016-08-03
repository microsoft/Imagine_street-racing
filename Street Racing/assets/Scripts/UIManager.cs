using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// User interface manager. This controls most of the UI logic for the game.
/// </summary>
public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[SerializeField]
	GameObject[] screens = {};

	[SerializeField]
	GameObject hud;

	[SerializeField]
	Text timeText = null;
	[SerializeField]
	Text speedText = null;
	[SerializeField]
	Text rankText = null;
	[SerializeField]
	Text countdownText = null;

	[SerializeField]
	AudioClip buttonSound = null;
	AudioSource buttonSource;

	[SerializeField]
	AudioClip countdownSound = null;
	AudioSource countdownSource;

	[SerializeField]
	AudioClip goSound = null;
	AudioSource goSource;

	void Awake()
	{
		Instance = this;
		buttonSource = AudioHelper.CreateAudioSource(gameObject, buttonSound);
		countdownSource = AudioHelper.CreateAudioSource(gameObject, countdownSound);
		goSource = AudioHelper.CreateAudioSource(gameObject, goSound);
	}

	/// <summary>
	/// Show a specific screen and hide others.
	/// </summary>
	/// <param name="name">Screen name.</param>
	public void ShowScreen(string name)
	{
		// Show the screen with the given name and hide everything else
		foreach (GameObject screen in screens)
		{
			screen.SetActive(screen.name == name);
		}
	}

	/// <summary>
	/// Shows/hides the HUD.
	/// </summary>
	/// <param name="show">Do we show the HUD?</param>
	public void ShowHUD(bool show)
	{
		hud.SetActive(show);
	}

	/// <summary>
	/// Call this to update the HUD elements.
	/// </summary>
	/// <param name="speed">Current speed.</param>
	/// <param name="time">Current time spent.</param>
	/// <param name="rank">The player's rank.</param>
	public void UpdateHUD(float speed, float time, Rank rank)
	{
		ShowTime(time);
		ShowSpeed((int)(speed * 10));
		ShowRank(rank);
	}

	/// <summary>
	/// Updates the score on the HUD.
	/// </summary>
	/// <param name="score">Score.</param>
	void ShowTime(float time)
	{
		TimeSpan tSpan = TimeSpan.FromSeconds(time);
		string timeString = String.Format("{0:00}:{1:00}:{2:000}", tSpan.Minutes, tSpan.Seconds, tSpan.Milliseconds);
		timeText.text = String.Format(LocalizationManager.Instance.GetString("HUD Time"), timeString);
	}

	/// <summary>
	/// Updates the score on the HUD.
	/// </summary>
	/// <param name="speed">Speed.</param>
	void ShowSpeed(int speed)
	{
		speedText.text = String.Format(LocalizationManager.Instance.GetString("HUD Speed"), speed);
	}

	/// <summary>
	/// Updates the rank on the HUD.
	/// </summary>
	/// <param name="rank">Rank.</param>
	void ShowRank(Rank rank)
	{
		string rankKey = "Rank " + rank.ToString();
		rankText.text = string.Format(LocalizationManager.Instance.GetString("HUD Rank"), LocalizationManager.Instance.GetString(rankKey));
	}

	/// <summary>
	/// Shows the countdown.
	/// </summary>
	/// <param name="countdown">Countdown.</param>
	public void ShowCountdown(float countdown)
	{
		// Show a number for countdown if it's not 0 yet
		if (countdown > 0)
		{
			countdownText.text = countdown.ToString();
			countdownSource.Play();
		}
		// Show "Go" for 0
		else if (countdown == 0)
		{
			countdownText.text = LocalizationManager.Instance.GetString("Go");
			goSource.Play();
		}
		// Hide otherwise
		else
		{
			countdownText.text = "";
		}
	}

	/// <summary>
	/// Called when a screen button is clicked.
	/// </summary>
	public void OnButtonClick()
	{
		buttonSource.Play();
	}

	public void OnLanguageChanged()
	{
		foreach (GameObject o in screens)
		{
			var staticText = o.GetComponent<StaticTextManager>();
			if (staticText)
			{
				staticText.OnLanguageChanged();
			}
		}
	}
}