using UnityEngine;
using System.Collections;

/// <summary>
/// GameplayManager - behaviour that controls general gameplay flow.
/// </summary>
public class GameplayManager : MonoBehaviour
{
	public static GameplayManager Instance;

	// Static -- allows us to detect if this is the first playthrough or not
	private static bool firstRun = true;

	[SerializeField]
	float horizontalBounds;
	public float HorizontalBounds
	{
		get
		{
			return horizontalBounds;
		}
		private set
		{
			horizontalBounds = value;
		}
	}

	[Range(1f, 10f)]
	[SerializeField]
	float minVerticalTopSpeed = 5f;
	[Range(2f, 20f)]
	[SerializeField]
	float maxVerticalTopSpeed = 10f;

	public float MaxVerticalTopSpeed
	{
		get
		{
			return maxVerticalTopSpeed;
		}
	}

	[Range(0f, 4f)]
	[SerializeField]
	float minVerticalObstacleSpeed = 4f;
	[Range(0f, 8f)]
	[SerializeField]
	float maxVerticalObstacleSpeed = 8f;

	float currentTime;
	int countdown;

	[SerializeField]
	float timeToStartDifficultyRamp = 5000;
	[SerializeField]
	float timeForMaxDifficulty = 50000;

	[SerializeField]
	float goldTimeReq = 70f;
	[SerializeField]
	float silverTimeReq = 50f;
	[SerializeField]
	float bronzeTimeReq = 30f;

	[SerializeField]
	AudioClip musicSound = null;
	AudioSource musicSource = null;

	public float CurrentZPos
	{
		get;
		set;
	}

	float currentSpeed;

	enum GameState
	{
		Tutorial,
		InGame,
		GameOver
	};

	GameState currentState;

	/// <summary>
	/// ExecuteInEditMode - this function will run in the editor.
	/// OnValidate - this function will run every time variables in this script change outside of this function.
	/// </summary>
	[ExecuteInEditMode]
	void OnValidate()
	{
		minVerticalTopSpeed = Mathf.Clamp(minVerticalTopSpeed, 0f, maxVerticalTopSpeed);
		maxVerticalTopSpeed = Mathf.Clamp(maxVerticalTopSpeed, minVerticalTopSpeed, 20f);
	}

	void Awake()
	{
		Instance = this;
		musicSource = AudioHelper.CreateAudioSource(gameObject, musicSound);
	}

	void Start()
	{
		currentState = GameState.Tutorial;
		countdown = -1;
		currentTime = 0f;
		currentSpeed = 0f;
		UIManager.Instance.UpdateHUD(currentSpeed, currentTime, Rank.Unranked);
		UIManager.Instance.ShowCountdown(countdown);

		// Show tutorial if this is the first time the game has been run this session
		if (firstRun)
		{
			firstRun = false;
			UIManager.Instance.ShowHUD(false);
			UIManager.Instance.ShowScreen("Tutorial");
		}
		else
		{
			OnStartGame();
		}
	}
	
	void Update()
	{
		// Update timer and HUD based on time
		if (currentState == GameState.InGame)
		{
			currentTime += Time.deltaTime;
			UIManager.Instance.UpdateHUD(currentSpeed, currentTime, GetRankForTime());
		}

		// Spacebar starts and resets game
		if (Input.GetKeyUp(KeyCode.Space))
		{
			if (currentState == GameState.GameOver)
			{
				OnRestartGame();
			}
			else if (currentState == GameState.Tutorial && countdown == -1)
			{
				OnStartGame();
			}
		}
	}

	/// <summary>
	/// CanUpdateGame - returns whether or not the caller should attempt to run update logic.
	/// </summary>
	/// <returns>True if the caller should run update logic, false otherwise.</returns>
	public bool CanUpdateGame()
	{
		// Game is effectively paused if not in-game
		return currentState == GameState.InGame;
	}

	public bool HasGameStarted()
	{
		return currentState != GameState.Tutorial;
	}

	/// <summary>
	/// Logic that runs when we actually start the game
	/// </summary>
	public void OnStartGame()
	{
		// Hide all screens except the HUD and start a countdown
		UIManager.Instance.ShowHUD(true);
		UIManager.Instance.ShowScreen("");
		countdown = 3;

		// Call ShowCountdown every 1 second, starting now
		InvokeRepeating("ShowCountdown", 0f, 1f);
	}

	/// <summary>
	/// Update the game start countdown timer, and start the game if it has finished.
	/// </summary>
	void ShowCountdown()
	{
		UIManager.Instance.ShowCountdown(countdown);

		if (countdown >= 0)
		{
			// Countdown is done
			if (countdown == 0)
			{
				currentState = GameState.InGame;
				musicSource.Play();
			}

			// Countdown should continue
			--countdown;
		}
		else
		{
			CancelInvoke("ShowCountdown");

			// We are done counting down, start the game!
			currentState = GameState.InGame;
		}
	}

	public void OnGameOver()
	{
		currentState = GameState.GameOver;
		musicSource.Stop();
		UIManager.Instance.ShowScreen("Game Complete");
	}

	public void OnRestartGame()
	{
		Application.LoadLevel(0);
	}

	public float GetDifficultyModifier()
	{
		// Time-based difficulty increase, linearly increasing from 0 to 1
		return Mathf.Clamp01((currentTime - timeToStartDifficultyRamp) / (timeForMaxDifficulty - timeToStartDifficultyRamp));
	}

	public float GetCurrentTopSpeed()
	{
		// Linearly increasing maximum speed
		return Mathf.Lerp(minVerticalTopSpeed, maxVerticalTopSpeed, GetDifficultyModifier());
	}

	public float GetCurrentObstacleSpeed()
	{
		// Linearly increasing obstacle speed
		return Mathf.Lerp(minVerticalObstacleSpeed, maxVerticalObstacleSpeed, GetDifficultyModifier());
	}

	Rank GetRankForTime()
	{
		Rank retRank = Rank.Unranked;

		if (currentTime >= goldTimeReq)
		{
			retRank = Rank.Gold;
		}
		else if (currentTime >= silverTimeReq)
		{
			retRank = Rank.Silver;
		}
		else if (currentTime >= bronzeTimeReq)
		{
			retRank = Rank.Bronze;
		}

		return retRank;
	}

	public void OnLanguageChanged()
	{
		UIManager.Instance.OnLanguageChanged();
		UIManager.Instance.UpdateHUD(currentSpeed, currentTime, GetRankForTime());
	}

	public void SetCurrentSpeed(float speed)
	{
		currentSpeed = speed;
	}
}
