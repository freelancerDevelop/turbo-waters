using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random=UnityEngine.Random;

[DisallowMultipleComponent]
public class Powerup : MonoBehaviour
{
    public PowerupType type = PowerupType.Speed;

    private GameObject body;
    private GameObject shadow;
    private GameObject glow;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Collider mainCollider;
    private ParticleSystem glowParticleSystem;

    private bool isActive = true;
    private Player collectedByPlayer = null;
    private float deathDuration = 1f;
    private float deathRemaining = 1f;

    public void Start()
    {
        this.mainCollider = this.GetComponent<Collider>();

        this.body = this.transform.Find("PowerupBody").gameObject;
        this.shadow = this.transform.Find("PowerupShadow").gameObject;
        this.glow = this.transform.Find("PowerupGlow").gameObject;

        this.meshFilter = this.body.GetComponent<MeshFilter>();
        this.meshRenderer = this.body.GetComponent<MeshRenderer>();
        this.glowParticleSystem = this.glow.GetComponent<ParticleSystem>();

        PowerupManager.Instance.AddPowerupColliderMapping(this, this.mainCollider);
    }

    public void Update()
    {
        if (this.isActive) {
            this.body.transform.localPosition = new Vector3(this.body.transform.localPosition.x, 0.6f + 0.05f * Mathf.Sin(Time.time * 2f), this.body.transform.localPosition.z);
            this.body.transform.localRotation = this.body.transform.localRotation * Quaternion.Euler(0, 0, -20f * Time.deltaTime);
            this.shadow.transform.localScale = Vector3.one * (0.1f + 0.02f * Mathf.Sin(Time.time * 2f));
            return;
        }

        this.deathRemaining -= Time.deltaTime;

        this.body.transform.localScale = Vector3.Lerp(this.body.transform.localScale, Vector3.zero, 0.2f);
        this.body.transform.localRotation = this.body.transform.localRotation * Quaternion.Euler(0, 0, -20f * Time.deltaTime);
        this.shadow.transform.localScale = Vector3.Lerp(this.shadow.transform.localScale, Vector3.zero, 0.2f);

        if (this.deathRemaining > 0) {
            return;
        }

        Destroy(this.gameObject);
    }

    public Collider GetCollider()
    {
        return this.mainCollider;
    }

    public bool IsActive()
    {
        return this.isActive;
    }

    public void CollectPowerup(Player player)
    {
        if (!this.isActive) {
            return;
        }

        this.isActive = false;
        this.collectedByPlayer = player;

        this.glowParticleSystem.Stop();

        if (this.type == PowerupType.Speed) {
            player.GiveSpeedBoost(5f);
        } else if (this.type == PowerupType.Expand) {
            player.GiveScaleBoost(5f);
        }
    }
}
