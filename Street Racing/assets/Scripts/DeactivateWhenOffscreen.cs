using UnityEngine;
using System.Collections;

/// <summary>
/// DeactivateWhenOffscreen - a behaviour that deactivates objects when they move below the camera's viewport.
/// </summary>
public class DeactivateWhenOffscreen : MonoBehaviour
{
	[SerializeField]
	float viewportDistanceOffscreen = 0.5f;

	void FixedUpdate()
	{
		var renderer = gameObject.GetComponentInChildren<Renderer>();
		if (renderer)
		{
			var top = renderer.bounds.max;
			top.x = 0;
			// Determine the vertical position of the uppermost rendered part of the object
			var viewportY = Camera.main.WorldToViewportPoint(top).y;

			// Objects with viewport position less than y = 0 are below the viewport
			if (viewportY < -viewportDistanceOffscreen)
			{
				// Deactivate object
				ObjectPooler.Instance.ReturnPooledObject(gameObject);
			}
		}
		else
		{
			// If it's not rendered it's offscreen to us
			ObjectPooler.Instance.ReturnPooledObject(gameObject);
		}
	}
}
