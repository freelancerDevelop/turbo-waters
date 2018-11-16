using UnityEngine;

public class ClamSpawner : Singleton<ClamSpawner> {

    public int MaximumClam = 60;
    public int TotalClam = 0;

    private void Update() {
        // Spawn to populate
        if (TotalClam < MaximumClam - 1) {
            SpawnClam();
        }
    }

    private void SpawnClam() {
        // Get position for clam
        Vector3 spawnPosition = new Vector3(Random.Range(-250f, 250f), 5, Random.Range(-250f, 250f));

        // Create clam GameObject
        GameObject obstacleObject = Instantiate(Resources.Load<GameObject>("Prefabs/Clam"), spawnPosition, Quaternion.identity);

        // Set obstacle properties
        obstacleObject.name = "Clam(" + TotalClam + ")";

        //Increment obstacles 
        TotalClam++;
    }

}

