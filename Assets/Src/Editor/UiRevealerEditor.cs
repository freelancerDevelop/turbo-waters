using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UiRevealer))]
public class UiRevealerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UiRevealer revealer = target as UiRevealer;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Reveal")) {
            revealer.Reveal();
        }

        GUI.enabled = true;
    }
}
