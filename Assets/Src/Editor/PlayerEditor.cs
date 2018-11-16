using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public void OnSceneGUI()
    {
        Player player = (Player) target;

        Handles.color = Color.blue;
    }
}
