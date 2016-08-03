using UnityEngine;
using System.Collections;

/// <summary>
/// RandomMaterial - script that allows the attached object's renderer to choose a material at random from a list.
/// </summary>
public class RandomMaterial : MonoBehaviour
{
	[SerializeField]
	Material[] materials = {};

	void OnEnable()
	{
		// Select random material to render if any are specified
		if (materials.Length > 0)
		{
			GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
		}
	}
}
