using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class FishBot : MonoBehaviour {

    private GameManager gameManager;
    private GameModeRunner gameRunner;
    private Player player;
    private PlayerMover mover;

    private Boids boids = new Boids();

    private static Vector3[] directionTable;

    private const int xCellDeviation = 5;
    private const int yCellDeviation = 5;

    private const float boidsMaxVelocity = 10.0f;

    private const float grimeyCellWeight = 1.0f;
    private const float extraGrimeyCellWeight = 15.0f;
    private const float grimeMaxAccelerationPerSecond = 10.0f;

    private const float adjustRandomWeightEverySeconds = 3.0f;
    private const float randomWeightMax = 0.5f;

    private const float minHardPointMultiplier = 0.5f;
    private const float maxHardPointMultiplier = 1.5f;

    private const float minEasyPointMultiplier = 0.5f;
    private const float maxEasyPointMultiplier = 0.7f;

    private const int timesLocalPlayerIsAllowedToDiePerHardGame = 3;
    private const int timesLocalPlayerIsAllowedToDiePerEasyGame = 1;

    private const int botsGetHardOnRound = 3;

    private int timesLocalPlayerIsAllowedToDiePerGame;
    private float randomWeightSeconds = 0;
    private Vector3 randomWeight;

    private float avoidSecondsSum;

    void Start() {
        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();
        this.player = this.GetComponent<Player>();
        this.mover = this.GetComponent<PlayerMover>();

        this.boids.SetMaxVelocity(boidsMaxVelocity);

        this.boids.AddWeighter(new BoidsWeighter() {
            weighter = this.WeightRandom,
            maxAccelerationPerSecond = randomWeightMax
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

        this.randomWeightSeconds = adjustRandomWeightEverySeconds;

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

    private Vector3 WeightRandom() {
        return this.randomWeight;
    }

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
        mover.MoveTowards(bp);
        mover.moving = true;
    }

    void Update() {
        if (this.gameRunner.GetState() == GameState.Countdown || this.gameRunner.GetState() == GameState.Ended) {
            return;
        }

        this.UpdateBoids();

        // Set shadow resolution
        //if (this.GetComponent<ShadowProjector>()._GlobalShadowResolution == Constants.SilhouetteResolution) {
        //    this.GetComponent<ShadowProjector>()._GlobalShadowResolution = Constants.SilhouetteResolution - 1;
        //} else {
        //    this.GetComponent<ShadowProjector>()._GlobalShadowResolution = Constants.SilhouetteResolution;
        //}
    }
}
