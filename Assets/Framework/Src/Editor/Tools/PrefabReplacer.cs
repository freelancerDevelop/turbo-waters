using UnityEngine;
using UnityEditor;

public class PrefabReplacer : EditorWindow
{
    [SerializeField]
    private GameObject prefab;

    [MenuItem("Tools/Prefab Replacer")]
    public static void ShowEditor()
    {
        EditorWindow.GetWindow<PrefabReplacer>();
    }

    private void OnGUI()
    {
        this.prefab = EditorGUILayout.ObjectField("Prefab", this.prefab, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Replace")) {
            GameObject[] selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i) {
                GameObject selected = selection[i];
                PrefabType prefabType = PrefabUtility.GetPrefabType(prefab);
                GameObject newObject;

                if (prefabType == PrefabType.Prefab) {
                    newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                } else {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                if (newObject == null) {
                    Logger.Error("Error instantiating prefab...");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");

                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

                Undo.DestroyObjectImmediate(selected);
            }
        }

        GUI.enabled = false;

        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
