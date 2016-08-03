using UnityEngine;
using System.Collections;

public class SectionManager : MonoBehaviour
{
	public static SectionManager Instance { get; private set; }

	[System.Serializable]
	public struct SectionInfo
	{
		public float weight;
		public GameObject objectToSpawn;
	}

	[SerializeField]
	SectionInfo[] sections = {};
	float totalWeight;

	bool generatingSection;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start()
	{
		generatingSection = false;

		// Accumulate weights for random selection
		foreach (SectionInfo s in sections)
		{
			totalWeight += s.weight;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		// Generate a section if we need to
		if (GameplayManager.Instance.CanUpdateGame() && !generatingSection)
		{
			SetupSection();
		}
	}

	/// <summary>
	/// SetupSection - Randomly selects a section to activate and start generating.
	/// </summary>
	void SetupSection()
	{
		// Randomly select a weight
		var selectedWeight = Random.Range(0f, totalWeight);

		GameObject obj = null;
		for (int i = 0; i < sections.Length; ++i)
		{
			// Subtract section weight from randomly selected weight
			selectedWeight -= sections[i].weight;

			// If we're at or below 0 weight, select this section
			if (selectedWeight <= 0)
			{
				obj = sections[i].objectToSpawn;
				break;
			}
		}

		// We didn't get a section... uh-oh
		if (!obj)
		{
			Debug.Break();
			return;
		}

		// Fetch pooled section
		var pooledSection = ObjectPooler.Instance.GetPooledObject(obj.name);
		// Move section to correct position
		pooledSection.GetComponent<ObstacleManager>().Setup(GameplayManager.Instance.CurrentZPos);
		// Activate
		pooledSection.SetActive(true);
		generatingSection = true;
	}

	public void OnSectionDone()
	{
		generatingSection = false;
	}
}
