using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : Singleton<FishSpawner> {

    public int MaximumFish = 120;
    public int TotalFish = 0;
    public int FishPerGroup = 10;
    public float GroupSpread = 3.5f;

    // Update is called once per frame
    private void Update() {
        // Spawn to populate
        if (TotalFish < MaximumFish - FishPerGroup) {
            SpawnFishGroup();
        }

        // Debug space-to-spawn testing
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    GameObject fishObject = Instantiate(Resources.Load<GameObject>("Prefabs/Fish1"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //}
    }

    private void SpawnFishGroup() {
        System.Random rnd = new System.Random();
        // Get position for full group
        List<Vector3> spawnZones = ((VanillaRunner)GameManager.Instance.GetRunner()).GetSpawnZones();
        int selectedZoneIndex = Random.Range(0, spawnZones.Count);
        Vector3 groupPosition = spawnZones[selectedZoneIndex] + new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));

        // Create fish for each fish in group
        for (int i = 0; i < FishPerGroup; i++) {
            // Get fish position based on group position
            Vector3 spawnPosition = new Vector3(groupPosition.x + Random.Range(-GroupSpread, GroupSpread), 0,
                                                groupPosition.z + Random.Range(-GroupSpread, GroupSpread));

            // Find random fish type index
            int fishType = rnd.Next(1, 5);

            // Create fish GameObject
            GameObject fishObject = Instantiate(Resources.Load<GameObject>("Prefabs/Fish" + fishType), spawnPosition, Quaternion.identity) as GameObject;
            //GameObject fishObject = Instantiate(Resources.Load<GameObject>("Prefabs/Fish1"), spawnPosition, Quaternion.identity) as GameObject;

            Player fish = fishObject.GetComponent<Player>();

            // Set fish properties
            fish.id = (uint) (TotalFish + i + 1);
            fish.name = "FishBot(" + fish.id + ")";
            fish.isLocalPlayer = false;

            GlobalProjectorManager.Get().OnShadowResolutionChange();
        }

        // Add to fish count
        TotalFish += FishPerGroup;
    }

}
