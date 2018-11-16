using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour, Destructible {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    public void Destruct() {
        GameObject splitObject = Instantiate(Resources.Load<GameObject>("Prefabs/ObstacleCut"), transform.position, Quaternion.identity);
        // Set random masses
        Rigidbody lcb = splitObject.transform.Find("LeftCube").GetComponent<Rigidbody>();
        Rigidbody rcb = splitObject.transform.Find("RightCube").GetComponent<Rigidbody>();
        lcb.mass = Random.Range(5f, 10f) * 10;
        rcb.mass = Random.Range(5f, 10f) * 10;

        // Set random bounce-up forces
        lcb.velocity = new Vector3(Random.Range(-3f, 3f), Random.Range(3f, 7f), Random.Range(-3f, 3f));
        rcb.velocity = new Vector3(Random.Range(-3f, 3f), Random.Range(3f, 7f), Random.Range(-3f, 3f));

        // Destroy original GameObject
        Destroy(gameObject);
    }

}
