using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PowerupManager : Singleton<PowerupManager>
{
    private Dictionary<Collider, Powerup> powerupColliderMapper = new Dictionary<Collider, Powerup>();

    public Powerup GetPowerupFromCollider(Collider collider)
    {
        if (this.powerupColliderMapper.ContainsKey(collider)) {
            return this.powerupColliderMapper[collider];
        }

        return null;
    }

    public void AddPowerupColliderMapping(Powerup powerup, Collider collider)
    {
        if (this.powerupColliderMapper.ContainsKey(collider)) {
            return;
        }

        this.powerupColliderMapper.Add(collider, powerup);
    }
}
