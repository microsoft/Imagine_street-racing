using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour
{
	public GameObject[] myPlanes;

	float planeEdgeZ;
	public float offscreenSpawnOffset = 100f;

	// Use this for initialization
	void Start()
	{
		// Start spawning further down so we get an edge at the bottom on start
		planeEdgeZ = -100f;
		SpawnNewPlane();
	}
	
	// Update is called once per frame
	void Update()
	{
		var maxZ = CameraController.GetMaxZ();

		// Spawn a new plane if we need to
		if (planeEdgeZ - maxZ <= offscreenSpawnOffset)
		{
			SpawnNewPlane();
		}
	}

	/// <summary>
	/// Randomly selects a plane to activate from the object pool and position in the world.
	/// </summary>
	void SpawnNewPlane()
	{
		// *** Add your source code here ***
	}
}
