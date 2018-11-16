using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Random=UnityEngine.Random;

[Serializable]
public class VanillaPlayerStats
{
    public uint id;
    public string name;
    public int points;
    public int kills;
}

public class VanillaRunner : GameModeRunner, ITimedMode
{
    protected int botsToSpawn = 7;
    protected float countdownRemaining = 3f;
    protected float nextPowerupIn = 5f;

    protected float nextBlockIn = 1f;
    protected int maximumBlocks = 20;
    protected int totalBlocks = 0;

    protected int selectedZoneIndex = 0;
    protected List<Vector3> spawnZones = new List<Vector3>();

    //private FishSpawner fishSpawner;
    //private ClamSpawner clamSpawner;
    //private ObstacleSpawner obstacleSpawner;
    //private EnemySpawner enemySpawner;

    public List<Player> players = new List<Player>();
    protected Dictionary<uint, VanillaPlayerStats> allPlayerStats = new Dictionary<uint, VanillaPlayerStats>();

    public override void Init()
    {
        base.Init();

        EventManager.Instance.AddListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
        EventManager.Instance.AddListener<PlayerKillsUpdatedEvent>(this.OnPlayerKillsUpdated);

        // Create Singleton spawners so they begin updating
        var fishSpawner = FishSpawner.Instance;
        var clamSpawner = ClamSpawner.Instance;
        var obstacleSpawner = ObstacleSpawner.Instance;
        var enemySpawner = EnemySpawner.Instance;

        // FIXME Temporary Called OnSceneLoad here
        OnSceneLoad();
    }

    public override void Destroy()
    {
        base.Destroy();

        if (EventManager.InstanceExists()) {
            EventManager.Instance.RemoveListener<PlayerPointsUpdatedEvent>(this.OnPlayerPointsUpdated);
            EventManager.Instance.RemoveListener<PlayerKillsUpdatedEvent>(this.OnPlayerKillsUpdated);
        }
    }

    public override void Update()
    {
        base.Update();

        // Decrease countdown
        if (this.state == GameState.Countdown) {
            this.countdownRemaining -= Time.deltaTime;

            if (this.countdownRemaining <= 0) {
                this.state = GameState.Game;
                this.countdownRemaining = 0;
            }

            return;
        }

        // Spawning random block object
        if (totalBlocks < maximumBlocks) {
            if (this.state == GameState.Game) {
                this.nextBlockIn -= Time.deltaTime;

                if (this.nextBlockIn <= 0) {
                    this.nextBlockIn = Random.Range(1.5f, 2.5f);
                    this.SpawnRandomObject();
                }
            }
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public float GetCountdownRemaining()
    {
        return this.countdownRemaining;
    }

    public Dictionary<uint, VanillaPlayerStats> GetAllPlayerStats()
    {
        return this.allPlayerStats;
    }

    public VanillaPlayerStats GetPlayerStats(uint playerId)
    {
        if (this.allPlayerStats.ContainsKey(playerId)) {
            return this.allPlayerStats[playerId];
        }

        return null;
    }

    public int GetRankForPlayer(uint playerId)
    {
        var sortedPlayerStats = this.allPlayerStats.Values.OrderByDescending(player => player.points);
        int rank = 0;

        foreach (VanillaPlayerStats p in sortedPlayerStats) {
            rank++;

            if (p.id == playerId) {
                return rank;
            }
        }

        return 1;
    }

    // FIXME Temporary OnSceneLoaded w/o Listener for simple testing
    private void OnSceneLoad()
    {
        this.state = GameState.Countdown;

        // Initialize all spawn zones
        this.spawnZones = new List<Vector3> {
            new Vector3(0, 0, 0)
        };

        this.selectedZoneIndex = Random.Range(0, this.spawnZones.Count);

        // Spawn player
        this.SpawnPlayer();

        // Spawn initial enemies
        EnemySpawner.Instance.SpawnInitialEnemies();
    }

    public void OnPlayerPointsUpdated(PlayerPointsUpdatedEvent e)
    {
        Logger.Message("[VanillaRunner] Player points updated event called for id: " + e.player.id);

        if (this.allPlayerStats.ContainsKey(e.player.id)) {
            this.allPlayerStats[e.player.id].name = e.player.name;
            this.allPlayerStats[e.player.id].points = e.points;
        } else {
            this.allPlayerStats.Add(e.player.id, new VanillaPlayerStats {
                id = e.player.id,
                name = e.player.name,
                points = e.points,
                kills = 0
            });
        }
    }

    private void OnPlayerKillsUpdated(PlayerKillsUpdatedEvent e)
    {
        if (this.allPlayerStats.ContainsKey(e.player.id)) {
            this.allPlayerStats[e.player.id].name = e.player.name;
            this.allPlayerStats[e.player.id].kills = e.kills;
        } else {
            this.allPlayerStats.Add(e.player.id, new VanillaPlayerStats {
                id = e.player.id,
                name = e.player.name,
                points = 0,
                kills = e.kills
            });
        }
    }

    private void SpawnPlayer()
    {
        GameObject mainCamera = GameObject.Find("CameraTarget");
        GameCamera gameCamera = mainCamera.GetComponent<GameCamera>();
        Vector3 spawnPosition = this.spawnZones[this.selectedZoneIndex] + new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));

        GameObject playerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), spawnPosition, Quaternion.identity);
        Player p = playerObject.GetComponent<Player>();
        playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.GetComponent<PlayerMover>().GetWaterLevel(), playerObject.transform.position.z);


        p.id = 1;
        p.name = StorageManager.Instance.GetString(StorageKeys.PlayerName, "LocalPlayer");
        p.Type = PlayerType.Active;

        gameCamera.target = playerObject;

        this.players.Add(p);

        this.OnPlayerPointsUpdated(new PlayerPointsUpdatedEvent {
            player = p,
            points = 0,
            gained = 0
        });
    }

    private void SpawnPowerup()
    {
        Logger.Message("Spawned power-up");

        Vector3 spawnPosition = new Vector3(Random.Range(0, 20f), 0, Random.Range(-20f, 0));
        PowerupType powerupType = (PowerupType) Random.Range(0, 2);
        string powerupName = "Unknown";

        if (powerupType == PowerupType.Speed) {
            powerupName = "Speed";
        } else if (powerupType == PowerupType.Expand) {
            powerupName = "Expand";
        }

        GameObject powerupObject = Instantiate(Resources.Load<GameObject>("Prefabs/Powerups/" + powerupName), spawnPosition, Quaternion.identity);
        Powerup powerup = powerupObject.GetComponent<Powerup>();
    }

    private void SpawnRandomObject()
    {
        // Get spawn position
        Vector3 spawnPosition = new Vector3(Random.Range(0, 20f), 3, Random.Range(-20f, 0));

        // Insantiate object
        GameObject randomObject = Instantiate(Resources.Load<GameObject>("Prefabs/RandomObject"), spawnPosition, Quaternion.identity);

        // Increase total blocks
        totalBlocks++;
    }

    public List<Vector3> GetSpawnZones() {
        return this.spawnZones;
    }
}
