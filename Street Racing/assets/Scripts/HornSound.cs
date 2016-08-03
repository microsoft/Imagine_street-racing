using UnityEngine;
using System.Collections;

public class HornSound : MonoBehaviour
{

	[SerializeField]
	AudioClip hornSound = null;
	AudioSource hornSource = null;

	[SerializeField]
	[Range(0f, 1f)]
	float playProbability = 1f;

	[SerializeField]
	[Range(0f, 200f)]
	float playDistance = 200f;

	bool played;

	// Use this for initialization
	void Start()
	{
		hornSource = AudioHelper.CreateAudioSource(gameObject, hornSound);
		played = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!played && transform.position.z <= GameplayManager.Instance.CurrentZPos + playDistance)
		{
			played = true;
			if (Random.Range(0f, 1f) < playProbability)
			{
				hornSource.Play();
			}
		}
	}
}
