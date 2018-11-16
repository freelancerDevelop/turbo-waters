using UnityEngine;

public class ObstacleSpawner : Singleton<ObstacleSpawner> {

    public int MaximumObstacles = 20;
    public int TotalObstacles = 0;

    private void Update() {
        // Spawn to populate
        if (TotalObstacles < MaximumObstacles - 1) {
            SpawnObstacle();
        }
    }

    private void SpawnObstacle() {
        // Get position for obstacle
        Vector3 spawnPosition = new Vector3(Random.Range(-250f, 250f), 5, Random.Range(-250f, 250f));

        // Create obstacle GameObject
        GameObject obstacleObject = Instantiate(Resources.Load<GameObject>("Prefabs/Obstacle"), spawnPosition, Quaternion.identity);

        // Set obstacle properties
        obstacleObject.name = "Obstacle(" + TotalObstacles + ")";

        //Increment obstacles 
        TotalObstacles++;
    }


}
