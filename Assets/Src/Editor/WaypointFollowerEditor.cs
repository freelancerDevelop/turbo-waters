using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WaypointFollower))]
public class WaypointFollowerEditor : Editor
{
    public void OnSceneGUI()
    {
        WaypointFollower waypointFollower = target as WaypointFollower;
        Rigidbody followerRigidBody = waypointFollower.GetComponent<Rigidbody>();
        Renderer followerRenderer = waypointFollower.GetComponent<Renderer>();
        Vector3 direction = (Quaternion.Euler(waypointFollower.rotationOffset) * waypointFollower.transform.forward).normalized;
        Vector3 meshCenter = waypointFollower.transform.TransformPoint(followerRigidBody.centerOfMass);
        float meshLargestSize = 2f;

        if (followerRenderer) {
            meshLargestSize = Mathf.Max(followerRenderer.bounds.extents.x, followerRenderer.bounds.extents.z);
        }

        EditorUtils.ArrowDebug(meshCenter, direction * (meshLargestSize + 1f), Color.yellow, 0.5f, 20f);
    }
}