using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
	float lastSpawnZ;
	float lengthToSpawn;
	float zProgress;

	[SerializeField]
	protected float spawnZOffset = 500f;
	[SerializeField]
	float minimumSectionLength = 100;
	[SerializeField]
	float maximumSectionLength = 1000;

	[SerializeField]
	float initialDistanceBetweenObstacles = 0f;
	float distanceBetweenObstacles;

	float distanceOffset = 0;
	float verticalSpeed;

	[Range(0.0f, 1.0f)]
	public float initialChanceToSpawn;
	[Range(0.0f, 1.0f)]
	public float maxChanceToSpawn;

	float chanceToSpawn;

	[System.Serializable]
	public struct SpawnChance
	{
		public float weight;
		public GameObject objectToSpawn;
	}

	[SerializeField]
	SpawnChance[] spawnChances = {};
	float totalWeight;

	[ExecuteInEditMode]
	void OnValidate()
	{
		// Make the tuning values make sense logically
		Mathf.Clamp(initialChanceToSpawn, 0, maxChanceToSpawn);
		Mathf.Clamp(maxChanceToSpawn, initialChanceToSpawn, 1f);
		Mathf.Clamp(minimumSectionLength, 0f, maximumSectionLength);
	}

	// Use this for initialization
	void Start()
	{
		chanceToSpawn = Mathf.Lerp(initialChanceToSpawn, maxChanceToSpawn, GameplayManager.Instance.GetDifficultyModifier());

		spawnZOffset += CameraController.GetVisibleRoadLength();

		foreach (SpawnChance c in spawnChances)
		{
			totalWeight += c.weight;
		}
	}
	
	public virtual void Setup(float startZPos)
	{
		lastSpawnZ = startZPos;
		zProgress = startZPos;
		lengthToSpawn = Random.Range(minimumSectionLength, maximumSectionLength) * (1 + GameplayManager.Instance.GetDifficultyModifier());
		distanceBetweenObstacles = initialDistanceBetweenObstacles * (1 + GameplayManager.Instance.GetDifficultyModifier());
		verticalSpeed = GameplayManager.Instance.GetCurrentObstacleSpeed();
		distanceOffset = 0;
	}

	// Update is called once per frame
	void Update()
	{
		// Update spawn probability based on difficulty modifier
		if (GameplayManager.Instance.CanUpdateGame())
		{
			chanceToSpawn = Mathf.Lerp(initialChanceToSpawn, maxChanceToSpawn, GameplayManager.Instance.GetDifficultyModifier());
			// Check section's z position relative to world z position
			var currentZ = GameplayManager.Instance.CurrentZPos;
			lengthToSpawn -= currentZ - zProgress;
			zProgress = currentZ;

			// If there is more section to spawn and we've gone far enough past the last spawned obstacle...
			if (lengthToSpawn > 0 && currentZ - lastSpawnZ >= distanceBetweenObstacles + distanceOffset)
			{
				// Update last attempted spawn position
				lastSpawnZ = currentZ;
				distanceOffset = 0;

				// Probability (chanceToSpawn) to spawn an object here
				if (Random.Range(0f, 1f) <= chanceToSpawn)
				{
					SpawnObject();
				}
			}
			// If we're out of spawn room and we don't need to buffer any more space for moving obstacles...
			else if (lengthToSpawn <= 0 && currentZ - lastSpawnZ >= distanceBetweenObstacles + distanceOffset)
			{
				// ...we're done this section
				SectionManager.Instance.OnSectionDone();
				ObjectPooler.Instance.ReturnPooledObject(gameObject);
			}
		}
	}

	void SpawnObject()
	{
		// *** Add your source code here ***
	}

	/// <summary>
	/// GetRandomPooledObjectByWeight - Randomly selects an object to spawn
	/// based on their relative weights in spawnChances and fetches a pooled object
	/// of the type.
	/// </summary>
	/// <returns>A GameObject from the pool.</returns>
	GameObject GetRandomPooledObjectByWeight()
	{
		// Pick a random number between 0 and totalWeight
		var rand = Random.Range(0f, totalWeight);
		for (int i = 0; i < spawnChances.Length; ++i)
		{
			// Subtract object weight from total weight
			rand -= spawnChances[i].weight;
			// If total weight went below 0, select this object
			if (rand < 0)
			{
				return ObjectPooler.Instance.GetPooledObject(spawnChances[i].objectToSpawn.name);
			}
		}

		// If this is hit, weights are 0 or there are no objects to spawn
		Debug.LogWarning("Could not find a random pooled object!");
		return null;
	}

	/// <summary>
	/// Calculates the length of road that an object will travel before going offscreen.
	/// </summary>
	/// <param name="obj">The object that is moving</param>
	/// <returns>The distance the object will travel vertically before going offscreen.</returns>
	float CalculateDistanceRequiredForObject(MovingObstacleScript obj)
	{
		// Moving obstacle moves off the screen at speed (carSpeed - obstacleSpeed)
		var speedDiff = GameplayManager.Instance.GetCurrentTopSpeed() - verticalSpeed;
		// Calculate time for object to move through the screen + the spawn buffer
		var time = (CameraController.GetVisibleRoadLength() + spawnZOffset) / speedDiff;
		// distance = speed * time
		return time * verticalSpeed;
	}
}
