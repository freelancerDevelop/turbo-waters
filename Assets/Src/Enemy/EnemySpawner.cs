using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner> {

    private int totalEnemyPlayers = 1;

    public void Update() {

    }

    public void SpawnInitialEnemies() {
        // Spawn all enemy players
        for (int i = 0; i < totalEnemyPlayers; i++) {
            SpawnEnemyPlayer();
        }
    }

    private void SpawnEnemyPlayer() {
        // Get spawn position
        VanillaRunner runner = ((VanillaRunner)(GameManager.Instance.GetRunner()));
        List<Vector3> spawnZones = runner.GetSpawnZones();
        int selectedZoneIndex = Random.Range(0, spawnZones.Count);
        Vector3 spawnPosition = spawnZones[selectedZoneIndex] + new Vector3(Random.Range(-40f, 8f), 0, Random.Range(-40f, 40f));

        // Instantiate enemy player
        GameObject playerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), spawnPosition, Quaternion.identity);
        playerObject.AddComponent<EnemyBot>();
        Player p = playerObject.GetComponent<Player>();
        playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.GetComponent<PlayerMover>().GetWaterLevel(), playerObject.transform.position.z);

        // Set player properties
        p.id = (uint) (runner.players.Count + 1);
        p.name = StorageManager.Instance.GetString(StorageKeys.PlayerName, "BotPlayer " + p.id);
        p.isLocalPlayer = false;
        p.Type = PlayerType.Active;

        // Add player to runner
        runner.players.Add(p);

        // Initial set of player points
        runner.OnPlayerPointsUpdated(new PlayerPointsUpdatedEvent {
            player = p,
            points = 0,
            gained = 0
        });
    }

}