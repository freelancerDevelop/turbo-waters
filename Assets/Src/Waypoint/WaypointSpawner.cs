using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random=UnityEngine.Random;

public class WaypointSpawner : MonoBehaviour
{
    public GameObject objectContainer;
    public int objectsToSpawn = 10;
    public bool respawnObjects = false;
    public List<GameObject> objectPool = new List<GameObject>();

    private WaypointPath waypointPath;
    private List<GameObject> trackedObjects = new List<GameObject>();
    private float checkForRespawnTimer = 2f;

    public void Start()
    {
        this.waypointPath = this.GetComponent<WaypointPath>();

        for (int i = 0; i < this.objectsToSpawn; i++) {
            this.SpawnObject();
        }
    }

    public void Update()
    {
        this.checkForRespawnTimer -= Time.deltaTime;

        if (this.checkForRespawnTimer > 0) {
            return;
        }

        for (int i = this.trackedObjects.Count - 1; i >= 0; i--) {
            if (this.trackedObjects[i] == null) {
                this.trackedObjects.RemoveAt(i);
            }
        }

        for (int i = this.trackedObjects.Count; i < this.objectsToSpawn; i++) {
            this.SpawnObject();
        }

        this.checkForRespawnTimer = 2f;
    }

    private void SpawnObject()
    {
        GameObject spawnedObject = Instantiate(this.objectPool[Random.Range(0, this.objectPool.Count)], this.objectContainer.transform);
        WaypointFollower spawnedFollower = spawnedObject.GetComponent<WaypointFollower>();
        List<Transform> waypoints = this.waypointPath.GetWaypoints();
        int lastWaypointIndex = Random.Range(0, waypoints.Count);
        int nextWaypointIndex = lastWaypointIndex + 1;

        if (nextWaypointIndex == waypoints.Count) {
            nextWaypointIndex = 0;
        }

        spawnedFollower.waypointPath = this.waypointPath;
        spawnedFollower.startWaypointIndex = nextWaypointIndex;

        Vector3 lastPoint = waypoints[lastWaypointIndex].position;
        Vector3 pathDirection = waypoints[nextWaypointIndex].position - lastPoint;
        Vector3 chosenPoint = lastPoint + pathDirection * (0.1f + 0.8f * Random.Range(0, pathDirection.magnitude) / pathDirection.magnitude);

        spawnedFollower.transform.position = chosenPoint;

        this.trackedObjects.Add(spawnedObject);
    }
}
