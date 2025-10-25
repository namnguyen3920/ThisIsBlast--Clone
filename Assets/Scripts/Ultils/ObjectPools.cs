using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPools : Singleton_Mono_Method<ObjectPools>
{
    [System.Serializable]
    public class Pool
    {
        public string poolTag;
        public GameObject prefab;
        public int poolSize;
        public Transform parentHolder;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Transform> parentHolderDictionary;

    void Start()
    {
        InitializePool();
    }
    private void InitializePool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        parentHolderDictionary = new Dictionary<string, Transform>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            Transform holder = pool.parentHolder;
            parentHolderDictionary.Add(pool.poolTag, holder);
            for (int i = 0; i < pool.poolSize; i++)
            {

                GameObject obj = Instantiate(pool.prefab, holder);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.poolTag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];
        Transform parentHolder = parentHolderDictionary[tag];

        if (objectPool.Count == 0)
        {
            Pool originalPool = pools.Find(p => p.poolTag == tag);
            if (originalPool != null)
            {
                GameObject newObj = Instantiate(originalPool.prefab);
                newObj.SetActive(false);
                objectPool.Enqueue(newObj);
            }
        }

        GameObject objectToSpawn = objectPool.Dequeue();

        
        objectToSpawn.transform.SetParent(parentHolder, false);
        
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Destroy(objectToReturn);
            return;
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(parentHolderDictionary[tag], false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
    public void ReturnToPool(string tag, GameObject objectToReturn, float delay)
    {
        StartCoroutine(ReturnAfterDelay(tag, objectToReturn, delay));
    }

    private IEnumerator ReturnAfterDelay(string tag, GameObject objectToReturn, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(tag, objectToReturn);
    }
}