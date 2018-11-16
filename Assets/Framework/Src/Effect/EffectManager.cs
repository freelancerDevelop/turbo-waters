using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public GameObject CreateAndParentTo(string name, Transform transform)
    {
        return this.CreateAndParentTo(name, transform, 1f);
    }

    public GameObject CreateAndParentTo(string name, Transform transform, float duration)
    {
        GameObject tempEffectObject = PoolManager.Instance.GetObject(name, transform);

        tempEffectObject.name = "EffectInParent<" + name + ">";

        //Util.FireAfterDelay(delegate {
            //PoolManager.Instance.ReleaseObject(tempEffectObject);
        //}, duration);

        return tempEffectObject;
    }
}
