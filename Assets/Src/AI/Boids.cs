using System;
using System.Collections.Generic;
using UnityEngine;

public struct BoidsWeighter
{
    public Func<Vector3> weighter;
    public float maxAccelerationPerSecond;
}

public class Boids {
    private List<BoidsWeighter> weighters = new List<BoidsWeighter>();
    private Vector3 velocity;
    private Vector3 lastAcceleration;
    private float maxVelocity;

    public void AddWeighter(BoidsWeighter weighter) {
        this.weighters.Add(weighter);
    }

    public void SetMaxVelocity(float maxVelocity) {
        this.maxVelocity = maxVelocity;
    }

    public Vector3 GetVelocity() {
        return this.velocity;
    }

    public Vector3 GetLastAcceleration() {
        return this.lastAcceleration;
    }

    public void Update() {
        Debug.Assert(this.weighters.Count > 0, "Must have some weighters before calling Update");
        Debug.Assert(this.maxVelocity > 0, "Must call SetMaxVelocity before calling Update");

        Vector3 finalDirection = Vector3.zero;
        for (int j = 0; j < this.weighters.Count; j++) {
            Vector3 unnormalizedDirection = this.weighters[j].weighter();

            if (unnormalizedDirection.magnitude >= 1) {
                finalDirection += unnormalizedDirection.normalized * this.weighters[j].maxAccelerationPerSecond * Time.deltaTime;
            } else {
                finalDirection += unnormalizedDirection * this.weighters[j].maxAccelerationPerSecond * Time.deltaTime;
            }
        }

        this.lastAcceleration = finalDirection;
        this.velocity += finalDirection;

        if (this.velocity.magnitude >= this.maxVelocity) {
            this.velocity = this.velocity.normalized * this.maxVelocity;
        }

        //Debug.LogFormat("Acceleration {0}, {1}", this.acceleration.x, this.acceleration.z);
    }
}
