using UnityEngine;

public class PoolableBehaviour : MonoBehaviour
{
    private string prefabPath = "Unknown";
    private bool isPooled = false;

    public string GetPrefabPath()
    {
        return this.prefabPath;
    }

    public void SetPrefabPath(string prefabPath)
    {
        this.prefabPath = prefabPath;
    }

    public bool GetIsPooled()
    {
        return this.isPooled;
    }

    public void SetIsPooled(bool isPooled)
    {
        this.isPooled = isPooled;
    }

    public void Release()
    {
        PoolManager.Instance.ReleaseObject(this.gameObject);
    }

    public virtual void Reset()
    {
        // Called every time the object is put back into the available
        // pool of prefabs. You can also hook into OnEnable and
        // OnDisable to perform other custom behaviour when the
        // object is pulled from the pool.
    }
}
