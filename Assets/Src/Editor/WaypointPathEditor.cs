using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
    private Waypoint[] waypoints;

    public void OnSceneGUI()
    {
        WaypointPath waypointPath = target as WaypointPath;

        if (waypointPath.GetComponent<Waypoint>() != null) {
            if (waypointPath.transform.parent != null) {
                this.waypoints = waypointPath.transform.parent.GetComponentsInChildren<Waypoint>();
            }
        } else {
            if (waypointPath.transform.childCount > 1) {
                this.waypoints = waypointPath.GetComponentsInChildren<Waypoint>();
            }
        }

        Handles.color = Color.green;

        if (this.waypoints == null) {
            return;
        }

        for (int i = 0; i < this.waypoints.Length; i++) {
            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.PositionHandle(this.waypoints[i].transform.position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this.waypoints[i].transform, "Move Waypoint");

                this.waypoints[i].transform.position = newPosition;
            }
        }

        if (this.waypoints.Length > 1) {
            for (int i = 0; i < this.waypoints.Length - 1; i++) {
                EditorUtils.TextHandle(this.waypoints[i].transform.position, i.ToString(), Color.white, Color.black, 32, 40f);

                Handles.DrawLine(this.waypoints[i].transform.position, this.waypoints[i + 1].transform.position);
            }

            Handles.color = Color.red;

            EditorUtils.TextHandle(this.waypoints[this.waypoints.Length - 1].transform.position, (this.waypoints.Length - 1).ToString(), Color.white, Color.black, 32, 40f);

            Handles.DrawLine(this.waypoints[this.waypoints.Length - 1].transform.position, this.waypoints[0].transform.position);
        }
    }
}
