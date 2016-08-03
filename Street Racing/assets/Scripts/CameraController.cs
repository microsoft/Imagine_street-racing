using UnityEngine;
using System.Collections;

 /// <summary>
 /// CameraController - A behaviour that contains logic for camera movement,
 /// and helper functions related to camera position.
 /// </summary>
public class CameraController : MonoBehaviour
{
	void Start()
	{
		Update();
	}

	void Update()
	{
		Vector3 pos = transform.position;
		// Fix x position of camera
		pos.x = 0f;
		transform.position = pos;

		//Camera follows attached object and we're only interested in the z-axis
		GameplayManager.Instance.CurrentZPos = pos.z;
	}

	/// <summary>
	/// Calculates and returns the length of road that is visible by the main camera.
	/// </summary>
	/// <returns>The length of the road visible by the main camera, in world units.</returns>
	public static float GetVisibleRoadLength()
	{
		// Plane y = 0 for our road
		Plane p = new Plane(new Vector3(0, 1, 0), 0f);
		// Ray from camera at the top of the screen (in the middle)
		Ray top = Camera.main.ViewportPointToRay(new Vector3(0.5f, 1f, 0f));
		// Ray from camera at the bottom of the screen (in the middle)
		Ray bottom = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0f, 0f));

		float topDistance = 0f, bottomDistance = 0f;
		// Raycast rays onto the plane and get the distance
		p.Raycast(top, out topDistance);
		p.Raycast(bottom, out bottomDistance);
		// Calculate where the rays intersected
		Vector3 topPos = top.GetPoint(topDistance);
		Vector3 bottomPos = bottom.GetPoint(bottomDistance);
		// Calculate z difference between top and bottom
		return topPos.z - bottomPos.z;
	}
	 /// <summary>
	 /// Calculates the world z-coordinate of the top of the viewport of the main camera.
	 /// </summary>
	 /// <returns>The z-coordinate (in world units) of the top of the camera.</returns>
	public static float GetMaxZ()
	{
		// Plane y = 0 for our road
		Plane p = new Plane(new Vector3(0, 1, 0), 0f);
		// Ray from camera at the top of the screen (in the middle)
		Ray top = Camera.main.ViewportPointToRay(new Vector3(0.5f, 1f, 0f));
		float topDistance = 0f;
		// Raycast to our plane
		p.Raycast(top, out topDistance);
		// Calculate where the ray intersected the plane
		Vector3 topPos = top.GetPoint(topDistance);
		return topPos.z;
	}
}
