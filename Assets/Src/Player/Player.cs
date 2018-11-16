using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using MoreMountains.NiceVibrations;
using TMPro;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{

    public PlayerState State = PlayerState.Swimming;
    public PlayerType Type = PlayerType.Passive;

    public uint id = 0;
    public bool isLocalPlayer = true;
    public bool isDead = false;
    public float deathTimer = 5f;
    public float points = 0;
    public float pointMultiplier = 1f;
    public int kills = 0;
    public int deaths = 0;

    [Header("Movement")]
    public LayerMask movementMask;
    public float moveSpeed = 2.5f;
    public float rotationSpeed = 360f;

    private Camera mainCamera;
    private GameCamera gameCamera;
    private GameManager gameManager;
    private GameModeRunner gameRunner;
    private PlayerManager playerManager;

    private Rigidbody mainRigidbody;
    private Collider boundsCollider;
    private PlayerAnimator animator;
    private PlayerMover mover;

    private Vector3 mousePosition = Vector3.zero;
    private int lastTier = 1;
    private float lastPointsGainedTime = 0;
    private float nextCrownUpdateIn = 0;
    private float speedMultiplier = 1f;

    private float speedRemaining = 0;
    private float scaleMultiplier = 1.35f;
    private float scaleRemaining = 0;
    private float scaleSpeed = 0.2f;

    private float targetY = 0;

    public void Start()
    {
        this.mainCamera = Camera.main;
        this.gameCamera = GameObject.Find("CameraTarget").GetComponent<GameCamera>();
        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();
        this.playerManager = PlayerManager.Instance;
        this.mousePosition = this.transform.position;

        this.mainRigidbody = this.GetComponent<Rigidbody>();
        this.boundsCollider = GameObject.Find("/Environment/Ground").GetComponent<Collider>();

        this.animator = this.transform.Find("Model") != null ? this.transform.Find("Model").GetComponent<PlayerAnimator>() : null;
        this.mover = (this.GetComponent<PlayerMover>() == null) ?
            this.gameObject.AddComponent<PlayerMover>() : this.GetComponent<PlayerMover>();

        if (this.isLocalPlayer) {
            this.gameRunner.SetPlayer(this);
        }
    }

    public void Update()
    {
        if (this.isDead) {
            this.deathTimer -= Time.deltaTime;

            if (this.deathTimer <= 0) {
                this.isDead = false;
                this.deathTimer = 5f;

                EventManager.Instance.Trigger(new PlayerSpawnEvent {
                    player = this
                });
            }
        }


        // Apply scale
        if (this.Type == PlayerType.Active) {
            // Get scale values
            float scale = this.GetScale();
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            float scaleZoom = 0.5f + (scale - 1f) * 0.5f;

            // Set camera zoom
            gameCamera.zoomRatio = Mathf.Lerp(gameCamera.zoomRatio, scaleZoom, 0.95f * Time.deltaTime);
            // Set player scale based on score
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, scaleVector, scaleSpeed);
        }

        // Death animation
        if (this.animator != null && this.isDead) {
            this.animator.SetMoving(false);
            return;
        }
    }

    public int GetTier() {
        int tier = 1;

        // Return tier based on current score
        if (this.points >= 1200f) {
            tier = 8;
        } else if (this.points >= 600f) {
            tier = 7;
        } else if (this.points >= 350f) {
            tier = 6;
        } else if (this.points >= 225f) {
            tier = 5;
        } else if (this.points >= 135f) {
            tier = 4;
        } else if (this.points >= 75f) {
            tier = 3;
        } else if (this.points >= 25f) {
            tier = 2;
        }

        return tier;
    }

    public float GetScale() {
        int tier = this.GetTier();

        if (tier != this.lastTier) {
            EventManager.Instance.Trigger(new PlayerTierUpdatedEvent {
                player = this,
                tier = tier
            });

            this.lastTier = tier;
        }

        if (tier == 1) {
            return 1f;
        }

        return Mathf.Min(3f, Mathf.Pow(1.05f, tier - 1) * this.scaleMultiplier);
    }

    public void AddPoints(float points) {
        this.points += points * this.pointMultiplier;

        EventManager.Instance.Trigger(new PlayerPointsUpdatedEvent {
            player = this,
            points = (int)this.points,
            gained = points * this.pointMultiplier
        });

        if (this.isLocalPlayer) {
            if (points * this.pointMultiplier >= 5f) {
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            } else if (points * this.pointMultiplier >= 2f && Time.time - this.lastPointsGainedTime > 0.1f) {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

                this.lastPointsGainedTime = Time.time;
            }
        }
    }

    public void AttackedBy(Player enemyPlayer) {
        if (this.isDead) {
            return;
        }

        this.deaths++;
        this.isDead = true;
        this.deathTimer = 5f;

        enemyPlayer.kills++;
        enemyPlayer.AddPoints(20f);

        EventManager.Instance.Trigger(new PlayerDeathEvent {
            player = this,
            eatenBy = enemyPlayer,
            deathTimer = this.deathTimer
        });

        // TEMP Destroy this player
        Destroy(gameObject);

        // Decrease total fish if necessary
        if (GetComponent<FishBot>() != null) {
            FishSpawner.Instance.TotalFish--;
        }
        // Otherwise, if a passive edible, decrease total of its type
        else if (this.gameObject.layer == LayerMask.NameToLayer("Edible")) {
            if (this.tag == "ClamEdible") {
                // Decrease clam edibles
            }
        }
    }

    public void GiveSpeedBoost(float duration) {
        this.speedMultiplier = 1.6f;
        this.speedRemaining = duration;
    }

    public void GiveScaleBoost(float duration) {
        this.scaleMultiplier = 1.6f;
    }

    public Camera GetMainCamera() {
        return this.mainCamera;
    }

    public GameCamera GetGameCamera() {
        return this.gameCamera;
    }

    public float GetSpeedMultiplier() {
        return this.speedMultiplier;
    }

    public Collider GetBoundsCollider() {
        return this.boundsCollider;
    }

    public PlayerAnimator GetAnimator() {
        return this.animator;
    }

    public Rigidbody GetMainRigidbody() {
        return this.mainRigidbody;
    }

}