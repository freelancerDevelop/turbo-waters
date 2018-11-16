using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private List<string> prefabsToPool = new List<string>();
    private Dictionary<string, Stack<GameObject>> pooledObjects = new Dictionary<string, Stack<GameObject>>();

    public void AddPoolablePrefab(string prefabPath, int quantity)
    {
        if (this.prefabsToPool.Contains(prefabPath)) {
            Logger.ErrorFormat("Unable to add poolable prefab '{0}' because the pool already exists...", prefabPath);
            return;
        }

        this.prefabsToPool.Add(prefabPath);

        this.pooledObjects[prefabPath] = new Stack<GameObject>();

        for (int i = 0; i < quantity; i++) {
            GameObject pooledObject = Instantiate(Resources.Load<GameObject>(prefabPath), this.transform);
            PoolableBehaviour poolableBehaviour = pooledObject.GetComponent<PoolableBehaviour>();

            if (poolableBehaviour == null) {
                poolableBehaviour = pooledObject.AddComponent<PoolableBehaviour>();
            }

            poolableBehaviour.SetPrefabPath(prefabPath);
            poolableBehaviour.SetIsPooled(true);

            pooledObject.SetActive(false);

            this.pooledObjects[prefabPath].Push(pooledObject);
        }
    }

    public GameObject GetObject(string prefabPath, Transform createAtTransform)
    {
        GameObject pooledObject;

        if (!this.prefabsToPool.Contains(prefabPath)) {
            Logger.MessageFormat("No pool exists for '{0}' but it was requested, creating a default pool...", prefabPath);

            this.AddPoolablePrefab(prefabPath, 1);
        }

        if (this.pooledObjects[prefabPath].Count > 0) {
            pooledObject = this.pooledObjects[prefabPath].Pop();

            pooledObject.SetActive(true);
            pooledObject.transform.SetParent(createAtTransform, false);

            pooledObject.GetComponent<PoolableBehaviour>().SetIsPooled(false);

            return pooledObject;
        }

        Logger.MessageFormat("No pooled object available for '{0}', consider increasing the pool size if you see this frequently...", prefabPath);

        pooledObject = Instantiate(Resources.Load<GameObject>(prefabPath), createAtTransform);

        PoolableBehaviour poolableBehaviour = pooledObject.GetComponent<PoolableBehaviour>();

        if (poolableBehaviour == null) {
            poolableBehaviour = pooledObject.AddComponent<PoolableBehaviour>();
        }

        poolableBehaviour.SetPrefabPath(prefabPath);
        poolableBehaviour.SetIsPooled(false);

        return pooledObject;
    }

    public void ReleaseObject(GameObject pooledObject)
    {
        if (pooledObject == null) {
            Logger.Message("Unable to release object back to pool because it no longer exists...");
            return;
        }

        PoolableBehaviour poolableBehaviour = pooledObject.GetComponent<PoolableBehaviour>();
        string prefabPath = poolableBehaviour.GetPrefabPath();

        if (poolableBehaviour.GetIsPooled()) {
            return;
        }

        poolableBehaviour.Reset();
        poolableBehaviour.SetIsPooled(true);

        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(this.transform, false);

        this.pooledObjects[prefabPath].Push(pooledObject);
    }

    public bool IsPoolableObject(string prefabPath)
    {
        return this.prefabsToPool.Contains(prefabPath);
    }
}
