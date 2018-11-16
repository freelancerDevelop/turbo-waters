using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerBot : MonoBehaviour {
    
    private GameManager gameManager;
    private GameModeRunner gameRunner;
    private Player player;
    private PlayerMover mover;

    private Boids boids = new Boids();
    //private Grime grime;

    private static Vector3[] directionTable;
    private static GameObject[] powerupsThisFrame;
    private static int powerupsThisFrameNumber;

    private static GameObject[] playersThisFrame;
    private static int playersThisFrameNumber;

    private static GameObject[] propsThisFrame;
    private static int propsThisFrameNumber;

    private const int xCellDeviation = 5;
    private const int yCellDeviation = 5;

    private const float boidsMaxVelocity = 10.0f;

    private const float grimeyCellWeight = 1.0f;
    private const float extraGrimeyCellWeight = 15.0f;
    private const float grimeMaxAccelerationPerSecond = 10.0f;

    private const float powerupWeight = 100.0f;
    private const float maxPowerupDistance = 2.8f;

    private const float dodgeEdgeWeight = 8.0f;
    private const float tooCloseToEdgeDistance = 1.0f;

    private const float adjustRandomWeightEverySeconds = 3.0f;
    private const float randomWeightMax = 0.5f;

    private const float dodgeBotsWeight = 8.0f;
    private const float tooCloseToBotsDistance = 3.0f;

    private const float propsWeightMax = 40.0f;
    private const float maxPropsDistance = 3.0f;

    private const float minHardPointMultiplier = 0.5f;
    private const float maxHardPointMultiplier = 1.5f;

    private const float minEasyPointMultiplier = 0.5f;
    private const float maxEasyPointMultiplier = 0.7f;

    private const float pvpWeightMax = 20.0f;
    private const float pvpAvoidMinWeight = 10.0f;
    private const float pvpAvoidMaxWeight = 10.0f;
    private const float pvpAttackMinWeight = 6.0f;
    private const float pvpAttackMaxWeight = 16.0f;
    private const float pvpAvoidDistance = 2.0f;
    private const float pvpAttackDistance = 2.0f;
    private const float pvpMinAttackSeconds = 3.0f;
    private const float pvpMaxAttackSeconds = 5.0f;
    private const float pvpMinAvoidSeconds = 3.0f;
    private const float pvpMaxAvoidSeconds = 5.0f;
    private const float pvpAvoidLocalPlayerWeight = 3.0f;

    private const int timesLocalPlayerIsAllowedToDiePerHardGame = 3;
    private const int timesLocalPlayerIsAllowedToDiePerEasyGame = 1;
    private const int minimumTimeBetweenDeathsForLocalPlayer = 30;

    private const int botsGetHardOnRound = 3;

    private int timesLocalPlayerIsAllowedToDiePerGame;
    private float randomWeightSeconds = 0;
    private Vector3 randomWeight;
    private int nearbyCells = 0;
    private float pvpAvoidWeight;
    private float pvpAttackWeight;
    private float avoidSeconds;
    private float attackSeconds;

    private float avoidSecondsSum;
    private float attackSecondsSum;

    void Start() {
        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();
        this.player = this.GetComponent<Player>();
        this.mover = this.GetComponent<PlayerMover>();
        //this.grime = GameObject.Find("Grime").GetComponent<Grime>();

        this.boids.SetMaxVelocity(boidsMaxVelocity);

        //this.boids.AddWeighter(new BoidsWeighter() {
        //    weighter = this.WeightGrime,
        //    maxAccelerationPerSecond = grimeMaxAccelerationPerSecond
        //});

        //this.boids.AddWeighter(new BoidsWeighter() {
        //    weighter = this.WeightPowerups,
        //    maxAccelerationPerSecond = powerupWeight
        //});

        //this.boids.AddWeighter(new BoidsWeighter() {
        //    weighter = this.WeightDodgeEdges,
        //    maxAccelerationPerSecond = dodgeEdgeWeight
        //});

        this.boids.AddWeighter(new BoidsWeighter() {
            weighter = this.WeightRandom,
            maxAccelerationPerSecond = randomWeightMax
        });

        this.boids.AddWeighter(new BoidsWeighter() {
            weighter = this.WeightDodgeBots,
            maxAccelerationPerSecond = dodgeBotsWeight
        });

        //this.boids.AddWeighter(new BoidsWeighter() {
        //    weighter = this.WeightProps,
        //    maxAccelerationPerSecond = propsWeightMax
        //});

        this.boids.AddWeighter(new BoidsWeighter() {
            weighter = this.WeightPvp,
            maxAccelerationPerSecond = pvpWeightMax
        });

        string prefix = "?";
        if (StorageManager.Instance.GetInt(StorageKeys.AllRounds, 0) >= botsGetHardOnRound - 1) {
            this.player.pointMultiplier = Random.Range(minHardPointMultiplier, maxHardPointMultiplier);
            this.timesLocalPlayerIsAllowedToDiePerGame = timesLocalPlayerIsAllowedToDiePerHardGame;
            prefix = "H";
        } else {
            this.player.pointMultiplier = Random.Range(minEasyPointMultiplier, maxEasyPointMultiplier);
            this.timesLocalPlayerIsAllowedToDiePerGame = timesLocalPlayerIsAllowedToDiePerEasyGame;
            prefix = "E";
        }

        this.pvpAvoidWeight = Random.Range(pvpAvoidMinWeight, pvpAvoidMaxWeight);
        this.pvpAttackWeight = Random.Range(pvpAttackMinWeight, pvpAttackMaxWeight);
        this.avoidSeconds = Random.Range(pvpMinAvoidSeconds, pvpMaxAvoidSeconds);
        this.attackSeconds = Random.Range(pvpMinAttackSeconds, pvpMaxAttackSeconds);

        this.randomWeightSeconds = adjustRandomWeightEverySeconds;

        this.player.name = prefix + "Bot(" + player.pointMultiplier.ToString("n2") + "x)";

        InitializeDirectionTable();
    }

    private Vector3 GetWashingPosition() {
        return this.transform.position;
    }

    private static void InitializeDirectionTable() {
        if (directionTable != null) {
            return;
        }

        int width = (xCellDeviation * 2) + 1;
        int height = (yCellDeviation * 2) + 1;

        directionTable = new Vector3[width * height];

        // Adding 1 here as a hack
        float maxDistance = (new Vector3(xCellDeviation + 1, 0, yCellDeviation + 1)).magnitude;

        for (int y = -yCellDeviation; y <= yCellDeviation; y++) {
            for (int x = -xCellDeviation; x <= xCellDeviation; x++) {
                int nonZeroX = x + xCellDeviation;
                int nonZeroY = y + yCellDeviation;

                Vector3 unnormalizedDirection = new Vector3(x, 0, y);

                float ratio = (unnormalizedDirection).magnitude / maxDistance;
                float reversedRatio = 1.0f - ratio;
                directionTable[nonZeroY * width + nonZeroX] = unnormalizedDirection.normalized * reversedRatio;
            }
        }
    }

    private Vector3 GetDirection(int x, int y) {
        int nonZeroX = x + xCellDeviation;
        int nonZeroY = y + yCellDeviation;
        int width = (xCellDeviation * 2) + 1;

        return directionTable[nonZeroY * width + nonZeroX];
    }

    private GameObject[] GetPowerupsThisFrame() {
        if (Time.frameCount == powerupsThisFrameNumber) {
            return powerupsThisFrame;
        }
        powerupsThisFrameNumber = Time.frameCount;
        powerupsThisFrame = GameObject.FindGameObjectsWithTag("Powerup");
        return powerupsThisFrame;
    }

    private GameObject[] GetPlayersThisFrame() {
        if (Time.frameCount == playersThisFrameNumber) {
            return playersThisFrame;
        }
        playersThisFrameNumber = Time.frameCount;
        playersThisFrame = GameObject.FindGameObjectsWithTag("Player");
        return playersThisFrame;
    }

    private GameObject[] GetPropsThisFrame() {
        if (Time.frameCount == propsThisFrameNumber) {
            return propsThisFrame;
        }
        propsThisFrameNumber = Time.frameCount;
        propsThisFrame = GameObject.FindGameObjectsWithTag("Prop");
        return propsThisFrame;
    }

    private Vector3 WeightPvp() {
        GameObject[] players = GetPlayersThisFrame();
        Vector3 weight = Vector3.zero;
        bool avoiding = false;
        bool attacking = false;
        for (int i = 0; i < players.Length; i++) {
            if (!players[i]) {
                continue;
            }

            Player otherPlayer = players[i].GetComponent<Player>();
            PlayerBot bot = players[i].GetComponent<PlayerBot>();
            if (bot == this) {
                continue;
            }
            if (bot) {
                // TODO: Remove me
                //continue;
            }

            if (!otherPlayer) {
                continue;
            }
            if (otherPlayer.isDead) {
                continue;
            }

            if (otherPlayer.GetTier() == player.GetTier()) {
                // Same tier, do nothing
                // WeightDodgeBots will handle movement if they are a bot
                continue;
            }

            Vector3 unnormalizedDirection = (otherPlayer.transform.position - transform.position);
            float length = unnormalizedDirection.magnitude;

            //bool shouldAvoidLocalPlayer = otherPlayer.isLocalPlayer && (
            //    (otherPlayer.timesKilled >= timesLocalPlayerIsAllowedToDiePerGame) ||
            //    (otherPlayer.lastTimeDied >= 0 && Time.time - otherPlayer.lastTimeDied <= minimumTimeBetweenDeathsForLocalPlayer)
            //);
            bool shouldAvoidLocalPlayer = false;

            if ((otherPlayer.GetTier() > player.GetTier() && this.avoidSecondsSum < this.avoidSeconds) || shouldAvoidLocalPlayer) {
                if (length >= pvpAvoidDistance) {
                    continue;
                }
                float ratio = 1.0f - length / pvpAvoidDistance;
                float avoidWeight = (shouldAvoidLocalPlayer ? pvpAvoidLocalPlayerWeight : pvpAvoidWeight);

                weight += unnormalizedDirection.normalized * -1.0f * length * avoidWeight;

                this.avoidSecondsSum += Time.deltaTime;
                avoiding = true;
            }
            if (otherPlayer.GetTier() < player.GetTier() && this.attackSecondsSum < this.attackSeconds && !shouldAvoidLocalPlayer) {
                if (length >= pvpAttackDistance) {
                    continue;
                }
                float ratio = 1.0f - length / pvpAttackDistance;

                weight += unnormalizedDirection.normalized * length * pvpAttackWeight;

                this.attackSecondsSum += Time.deltaTime;
                attacking = true;
            }
        }
        if (!avoiding) {
            this.avoidSecondsSum = Math.Max(this.avoidSecondsSum - Time.deltaTime, 0);
        }
        if (!attacking) {
            this.attackSecondsSum = Math.Max(this.attackSecondsSum - Time.deltaTime, 0);
        }
        return weight;
    }

    //private Vector3 WeightProps() {
    //    Vector3 weight = Vector3.zero;
    //    GameObject[] props = GetPropsThisFrame();
    //    for (int i = 0; i < props.Length; i++) {
    //        if (!props[i]) {
    //            continue;
    //        }
    //        Vector3 unnormalizedDirection = (props[i].transform.position - GetWashingPosition());
    //        float distance = unnormalizedDirection.magnitude;
    //        if (distance >= maxPropsDistance) {
    //            continue;
    //        }

    //        Prop prop = props[i].GetComponent<Prop>();
    //        if (!prop) {
    //            continue;
    //        }
    //        if (!prop.IsActive()) {
    //            continue;
    //        }
    //        if (prop.GetRequiredTier() > player.GetTier()) {
    //            continue;
    //        }

    //        float ratio = 1.0f - distance / maxPropsDistance;
    //        weight += unnormalizedDirection.normalized * propsWeightMax * ratio;
    //    }
    //    return weight;
    //}

    private Vector3 WeightDodgeBots() {
        GameObject[] players = GetPlayersThisFrame();
        Vector3 weight = Vector3.zero;
        for (int i = 0; i < players.Length; i++) {
            if (!players[i]) {
                continue;
            }

            PlayerBot bot = players[i].GetComponent<PlayerBot>();
            if (!bot) {
                continue;
            }
            if (bot == this) {
                continue;
            }
            Player otherPlayer = players[i].GetComponent<Player>();
            if (otherPlayer.GetTier() != player.GetTier()) {
                // Only dodge bots of the same level. WeightPvp
                // will handle all other bots
                continue;
            }
            if (otherPlayer.isDead) {
                continue;
            }
            Vector3 unnormalizedDirection = (GetWashingPosition() - bot.GetWashingPosition());
            float distance = unnormalizedDirection.magnitude;
            if (distance >= tooCloseToBotsDistance) {
                continue;
            }
            float ratio = 1.0f - distance / tooCloseToBotsDistance;
            weight += unnormalizedDirection.normalized * dodgeBotsWeight * ratio;
        }
        return weight;
    }

    private Vector3 WeightRandom() {
        return this.randomWeight;
    }

    private Vector3 WeightDodgeEdges() {
        Vector3 position = GetWashingPosition();
        Vector3 weight = Vector3.zero;
        if (position.x <= tooCloseToEdgeDistance) {
            weight.x += 1;
        }
        if (position.x >= 20 - tooCloseToEdgeDistance) {
            weight.x -= 1;
        }

        if (position.z >= -tooCloseToEdgeDistance) {
            weight.z -= 1;
        }
        if (position.z <= -20 + tooCloseToEdgeDistance) {
            weight.z += 1;
        }

        return weight;
    }

    //private Vector3 WeightPowerups() {
    //    Vector3 position = GetWashingPosition();

    //    Vector3 weight = Vector3.zero;
    //    GameObject[] powerups = GetPowerupsThisFrame();
    //    for (int i = 0; i < powerups.Length; i++) {
    //        if (!powerups[i]) {
    //            continue;
    //        }
    //        Powerup powerup = powerups[i].GetComponent<Powerup>();
    //        if (!powerup.IsActive()) {
    //            continue;
    //        }
    //        if (powerup.IsPlayerOnly()) {
    //            continue;
    //        }

    //        Vector3 direction = powerups[i].transform.position - GetWashingPosition();

    //        float distance = direction.magnitude;
    //        if (distance >= maxPowerupDistance) {
    //            continue;
    //        }

    //        float ratio = 1.0f - distance / maxPowerupDistance;
    //        weight += direction * powerupWeight * ratio;
    //    }
    //    return weight;
    //}

    //private Vector3 WeightGrime() {
    //    Vector3 position = GetWashingPosition();
    //    float widthPerCell = this.grime.GetWidthPerCell();
    //    float heightPerCell = this.grime.GetHeightPerCell();
    //    Vector3 weight = Vector3.zero;

    //    this.nearbyCells = 0;
    //    for (int y = -yCellDeviation; y <= yCellDeviation; y++) {
    //        for (int x = -xCellDeviation; x <= xCellDeviation; x++) {
    //            Vector3 delta = new Vector3(x * widthPerCell, 0, y * heightPerCell);
    //            Vector3 gridPosition = position + delta;
    //            int gridIndex = this.grime.WorldPositionToGridIndex(gridPosition, false);
    //            if (gridIndex == -1) {
    //                // Don't double count cells
    //                continue;
    //            }

    //            float cellWeight = 0;
    //            if (this.grime.IsGrimeyCell(gridIndex)) {
    //                cellWeight = grimeyCellWeight;
    //            }
    //            if (this.grime.IsExtraGrimeyCell(gridIndex)) {
    //                cellWeight = extraGrimeyCellWeight;
    //            }

    //            if (cellWeight > 0) {
    //                Vector3 direction = GetDirection(x, y);
    //                weight += GetDirection(x, y) * cellWeight;

    //                this.nearbyCells++;
    //            }
    //        }
    //    }

    //    return weight;
    //}

    void UpdateBoids() {
        this.randomWeightSeconds += Time.deltaTime;
        if (this.randomWeightSeconds >= adjustRandomWeightEverySeconds) {
            this.randomWeightSeconds = 0;
            float min = -randomWeightMax / 2;
            float max = randomWeightMax / 2;

            this.randomWeight = new Vector3(Random.Range(min, max), 0, Random.Range(min, max));
        }

        this.boids.Update();

        //this.player.WashTowards(GetWashingPosition() + this.boids.GetVelocity() * 1000);

        Vector3 bp = this.player.transform.position + (this.boids.GetVelocity() * 0.5f);
        //this.player.transform.position = bp;
        mover.MoveTowards(bp);
    }

    void Update() {
        if (this.gameRunner.GetState() == GameState.Countdown || this.gameRunner.GetState() == GameState.Ended) {
            return;
        }

        this.UpdateBoids();
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos() {
//        Gizmos.color = Color.red;
//        Gizmos.DrawRay(GetWashingPosition() + Vector3.up * 2, this.boids.GetVelocity());

//        Gizmos.color = Color.green;
//        Gizmos.DrawRay(GetWashingPosition() + Vector3.up * 2, this.boids.GetLastAcceleration());

//        GUIStyle style = new GUIStyle();
//        style.normal.textColor = Color.red;
//        Handles.Label(GetWashingPosition() + Vector3.up * 2, this.nearbyCells.ToString() + " cells", style);

//        Handles.Label(GetWashingPosition() + Vector3.up * 2 + Vector3.right * 2, "tier " + this.player.GetTier(), style);

//        Handles.Label(GetWashingPosition() + Vector3.up * 2 + Vector3.left * 2, "attack " + ((avoidSecondsSum / avoidSeconds) * 100.0f) + "%", style);

//        Vector3 size = new Vector3((xCellDeviation * 2 + 1) * this.grime.GetWidthPerCell(), 2.0f, (yCellDeviation * 2 + 1) * this.grime.GetHeightPerCell());

//        if (player.isDead == false) {
//            Gizmos.color = Color.green;
//        } else {
//            Gizmos.color = Color.red;
//        }
//        Gizmos.DrawWireCube(GetWashingPosition(), size);

//    }
//#endif
}
