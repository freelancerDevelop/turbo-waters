using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteAttacker : MonoBehaviour
{

    void Start()
    {
        //preyList = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Edible")
        {
            //preyList.Add(other.gameObject);
        }
    }

    public void ClearPreyList()
    {
        //preyList.Clear();
    }
}
