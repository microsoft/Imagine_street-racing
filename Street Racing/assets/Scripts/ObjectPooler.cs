using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
	public static ObjectPooler Instance;

	[System.Serializable]
	public struct PoolableObject
	{
		// Number of objects instantiated and stored on startup
		public int pooledAmount;
		// Object to instantiate
		public GameObject obj;
	}

	[SerializeField]
	List<PoolableObject> poolDefinitions = new List<PoolableObject>();

	Dictionary<string, List<GameObject>> poolDictionary;
	Dictionary<string, GameObject> prefabDictionary;

	void Awake()
	{
		Instance = this;

		poolDictionary = new Dictionary<string, List<GameObject>>();
		prefabDictionary = new Dictionary<string, GameObject>();
		
		foreach(PoolableObject p in poolDefinitions)
		{
			// *** Remove this line ***
			prefabDictionary.Add(p.obj.name, p.obj);
			// *** Add your source code here ***
		}
	}

	/// <summary>
	/// GetPooledObject - retrieves an inactive pooled object, or instantiates
	/// a new object and returns it.
	/// </summary>
	/// <param name="name">The name of the object.</param>
	/// <returns>An inactive pooled object, or null if the object type is not in the pool.</returns>
	public GameObject GetPooledObject(string name)
	{
		// *** Remove this line ***
		return CreatePooledObject(prefabDictionary[name]);
		// *** Add your source code here ***
	}

	/// <summary>
	/// ReturnPooledObject - returns a GameObject to the pool.
	/// </summary>
	/// <param name="obj">The object to return to the pool.</param>
	public void ReturnPooledObject(GameObject obj)
	{
		// *** Remove this line ***
		Destroy(obj);
		// *** Add your source code here ***
	}

	/// <summary>
	/// Creates a new object from a prefab, deactivates it, and returns it.
	/// </summary>
	/// <param name="prefab">The GameObject to instantiate.</param>
	/// <returns>The deactivated, instantiated game object.</returns>
	GameObject CreatePooledObject(GameObject prefab)
	{
		GameObject obj = (GameObject)Instantiate(prefab);
		obj.name = prefab.name;
		// Default object to inactive
		obj.SetActive(false);
		return obj;
	}
}