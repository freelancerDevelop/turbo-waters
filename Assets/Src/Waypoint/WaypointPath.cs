using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    private List<Transform> waypoints;

    public List<Transform> GetWaypoints()
    {
        if (this.waypoints == null) {
            this.waypoints = new List<Transform>();

            foreach (Transform child in this.transform) {
                this.waypoints.Add(child);
            }
        }

        return this.waypoints;
    }
}
