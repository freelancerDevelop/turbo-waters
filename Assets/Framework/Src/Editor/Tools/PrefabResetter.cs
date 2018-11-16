using UnityEngine;
using UnityEditor;

public class PrefabResetter : EditorWindow
{
    [MenuItem("Tools/Prefab Resetter")]
    public static void ShowEditor()
    {
        EditorWindow.GetWindow<PrefabResetter>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reset")) {
            GameObject[] selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i) {
                GameObject selected = selection[i];

                Undo.RegisterCompleteObjectUndo(selected, "Reset Prefabs");

                if (PrefabUtility.RevertPrefabInstance(selected)) {
                    Logger.MessageFormat("Successfully reset prefab: {0}", selected.name);
                } else {
                    Logger.ErrorFormat("Failed to reset prefab: {0}", selected.name);
                }

                Undo.FlushUndoRecordObjects();
            }
        }

        GUI.enabled = false;

        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
