using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class ScreenshotEditor : EditorWindow
{
    private string m_fileLocation;
    private string m_fileName;
    private int superSize = 1;

    [MenuItem("Tools/Screenshot Editor %l")]
    public static void ShowEditor()
    {
        ScreenshotEditor editor = EditorWindow.GetWindow<ScreenshotEditor>();

        editor.titleContent = new GUIContent("Screenshot");
        editor.Init();
    }

    private void Init()
    {
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Name the screenshot. Do not include file types", EditorStyles.helpBox);
        GUILayout.BeginHorizontal();

        m_fileName = EditorGUILayout.TextField("File Name: ", m_fileName);

        if (GUILayout.Button("Clear File Name", EditorStyles.miniButtonRight)) {
            m_fileName = "";
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("What Size do you want? 1 = Normal Camera Resolution", EditorStyles.helpBox);
        this.superSize = EditorGUILayout.IntField("Size of screenshot", this.superSize);

        GUILayout.Space(10);

        GUILayout.Label("Where do you want to save this screenshot?", EditorStyles.helpBox);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Select Save Location", EditorStyles.miniButtonLeft)) {
            m_fileLocation = EditorUtility.OpenFolderPanel( "Save Location", "", "" );
        }

        if (GUILayout.Button("Clear Save Location", EditorStyles.miniButtonRight)) {
            m_fileLocation = "";
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("This will take some time to write to a file. Be patient.", EditorStyles.helpBox);

        if (GUILayout.Button("Capture Screenshot")) {
            CaptureScreen();

            Debug.Log("Screenshot taken! May take time to write to disk depending on how big it is.");
            Debug.Log("Save Location: " + m_fileLocation + "/" + m_fileName + ".png");
        }
    }

    public void CaptureScreen()
    {
        ScreenCapture.CaptureScreenshot(m_fileLocation + "/" + m_fileName + ".png", this.superSize);
    }

    public void PlaymodeChanged()
    {
        this.Repaint();
    }

    public void OnLostFocus()
    {
        this.Repaint();
    }

    public void OnFocus()
    {
        this.Repaint();
    }

    public void OnProjectChange()
    {
        this.Repaint();
    }

    public void OnSelectionChange()
    {
        this.Repaint();
    }
}
