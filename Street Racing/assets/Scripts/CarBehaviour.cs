using UnityEngine;
using System.Collections;

/// <summary>
/// CarBehaviour - the behaviour that controls car movement and collision.
/// </summary>
public class CarBehaviour : MonoBehaviour
{
	Rigidbody rb;

	[Range(1f,10f)]
	[SerializeField]
	float horizontalSpeed = 5f;

	float currentVerticalSpeed;

	[SerializeField]
	float accelerationTime = 2f;

	[SerializeField]
	AudioClip carSound = null;
	AudioSource carSource = null;

	[SerializeField]
	[Range(-3f, 3f)]
	float minSpeedPitch;
	[SerializeField]
	[Range(-3f, 3f)]
	float maxSpeedPitch;

	[SerializeField]
	AudioClip crashSound = null;
	AudioSource crashSource = null;

	void OnValidate()
	{
		minSpeedPitch = Mathf.Clamp(minSpeedPitch, -3f, maxSpeedPitch);
		maxSpeedPitch = Mathf.Clamp(maxSpeedPitch, minSpeedPitch, 3f);
	}

	void Start()
	{
		currentVerticalSpeed = 0;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = rb.position - new Vector3(0, 0, 1);
		carSource = AudioHelper.CreateAudioSource(gameObject, carSound);
		crashSource = AudioHelper.CreateAudioSource(gameObject, crashSound);
	}

	void FixedUpdate()
	{
		if (GameplayManager.Instance.CanUpdateGame())
		{
			// Handle acceleration at beginning of game
			var speedIncrease = GameplayManager.Instance.GetCurrentTopSpeed() * Time.fixedDeltaTime / accelerationTime;
			// Clamp speed to current allowed top speed
			currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + speedIncrease, 0f, GameplayManager.Instance.GetCurrentTopSpeed());
			// Move car
			rb.position += new Vector3(0 ,0, currentVerticalSpeed);
			// Update GameplayManager with speed
			GameplayManager.Instance.SetCurrentSpeed(currentVerticalSpeed);

			if (!carSource.isPlaying)
			{
				carSource.Play();
			}

			carSource.pitch = Mathf.Lerp(minSpeedPitch, maxSpeedPitch, currentVerticalSpeed / GameplayManager.Instance.MaxVerticalTopSpeed);
		}
		else
		{
			carSource.Stop();
		}
	}

	void Update()
	{
		if (GameplayManager.Instance.CanUpdateGame())
		{
			// Process left and right car movement
			Vector3 pos = rb.position;

			pos.x += (Input.GetAxis("Horizontal") * horizontalSpeed);

			pos.x = Mathf.Clamp(pos.x, -GameplayManager.Instance.HorizontalBounds, GameplayManager.Instance.HorizontalBounds);
			rb.position = pos;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// We only care if the car hits obstacles on the road -- colliding with anything else is fine
		if (other.CompareTag("Obstacle"))
		{
			CrashCar(other);
			GameplayManager.Instance.OnGameOver();
		}
	}

	/// <summary>
	/// Turns on physics and adds explosive force to the car to simulate a spinout.
	/// </summary>
	/// <param name="other">The object we collided with.</param>
	void CrashCar(Collider other)
	{
		// *** Add your source code here ***
	}
}
