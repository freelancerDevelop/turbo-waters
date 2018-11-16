using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WaypointJunction : MonoBehaviour
{
    public WaypointJunctionState state = WaypointJunctionState.Green;
    public int startLane = 0;
    public float greenLightDuration = 5f;
    public float yellowLightDuration = 2f;

    private List<WaypointJunctionLane> lanes = new List<WaypointJunctionLane>();
    private float currentTimer = 0;
    private int currentLane = 0;

    public void Start()
    {
        this.currentLane = this.startLane;

        if (this.state == WaypointJunctionState.Green) {
            this.currentTimer = this.greenLightDuration;
        } else if (this.state == WaypointJunctionState.Yellow) {
            this.currentTimer = this.yellowLightDuration;
        }

        foreach (Transform child in this.transform) {
            this.lanes.Add(child.GetComponent<WaypointJunctionLane>());
        }
    }

    public void Update()
    {
        this.currentTimer -= Time.deltaTime;

        if (this.currentTimer > 0) {
            return;
        }

        if (this.state == WaypointJunctionState.Green) {
            this.lanes[this.currentLane].isFree = false;
            this.lanes[this.currentLane].isWaiting = true;

            this.state = WaypointJunctionState.Yellow;
            this.currentTimer = this.yellowLightDuration;
            return;
        }

        if (this.state == WaypointJunctionState.Yellow) {
            this.lanes[this.currentLane].isWaiting = true;

            this.currentLane++;

            if (this.currentLane == this.lanes.Count) {
                this.currentLane = 0;
            }

            this.lanes[this.currentLane].isFree = true;
            this.lanes[this.currentLane].isWaiting = false;

            this.state = WaypointJunctionState.Green;
            this.currentTimer = this.greenLightDuration;
            return;
        }
    }
}
