using UnityEngine;
using System.Collections;

public class MovingObstacleScript : MonoBehaviour
{
	[Range(0f, 10f)]
	public float horizontalSpeed = 0f;
	
	float verticalSpeed = 0f;

	[Range(0f, 0.1f)]
	public float chanceToSwitchDirection = 0.05f;
	const float timeToSwitchDirection = 0.5f;
	float switchTime;
	bool switchingDirection;
	bool movingLeft;

	[SerializeField]
	AudioClip carAudio = null;
	AudioSource carSource = null;
	
	void Start()
	{
		carSource = AudioHelper.CreateAudioSource(gameObject, carAudio);
	}

	void FixedUpdate()
	{
		if (GameplayManager.Instance.HasGameStarted())
		{
			if (!carSource.isPlaying)
			{
				carSource.Play();
			}

			Vector3 positionDelta = new Vector3(0, 0, verticalSpeed);

			// 0.5 second delay when switching directions
			if (switchingDirection)
			{
				switchTime -= Time.fixedDeltaTime;
				if (switchTime <= 0f)
				{
					switchingDirection = false;
					movingLeft = !movingLeft;
					switchTime = 0f;
				}
			}

			if (!switchingDirection)
			{
				// Random chance to switch directions each update
				if (Random.Range(0f, 1f) <= chanceToSwitchDirection)
				{
					switchingDirection = true;
					switchTime = timeToSwitchDirection;
				}
				else
				{
					// Move horizontally
					positionDelta.x += horizontalSpeed * (movingLeft ? -1 : 1);
				}
			}

			var newPosition = gameObject.transform.position += positionDelta;
			var width = GetComponentInChildren<Collider>().bounds.extents.x;

			// Clamp position to road
			var newPositionClamped = new Vector3(Mathf.Clamp(newPosition.x, width - GameplayManager.Instance.HorizontalBounds, GameplayManager.Instance.HorizontalBounds - width), newPosition.y, newPosition.z);

			// If we had to clamp, we hit one side and need to switch directions
			if (newPositionClamped != newPosition)
			{
				switchingDirection = true;
				switchTime = timeToSwitchDirection;
			}

			gameObject.transform.position = newPositionClamped;
		}
		else
		{
			carSource.Stop();
		}
	}

	public void SetVerticalSpeed(float speed)
	{
		verticalSpeed = speed;
	}
}
