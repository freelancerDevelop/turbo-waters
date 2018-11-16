using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class EditorUtils
{
    public static int arrowSegments = 32;

    public static void ArrowGizmo(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
    {
        Gizmos.DrawRay(position, direction);

        float arrowSegmentWidth = arrowHeadAngle / arrowSegments * 2f;

        for (int i = -arrowSegments / 2; i <= arrowSegments / 2; i++) {
            Vector3 offset = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f + i * arrowSegmentWidth, 0) * new Vector3(0, 0, 1f);

            Gizmos.DrawRay(position + direction, offset * arrowHeadLength);
        }
    }

    public static void ArrowDebug(Vector3 position, Vector3 direction, Color? color = null, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
    {
        if (color != null) {
            Debug.DrawRay(position, direction, (Color) color);
        } else {
            Debug.DrawRay(position, direction);
        }

        float arrowSegmentWidth = arrowHeadAngle / arrowSegments * 2f;

        for (int i = -arrowSegments / 2; i <= arrowSegments / 2; i++) {
            Vector3 offset = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180f + i * arrowSegmentWidth, 0) * new Vector3(0, 0, 1f);

            if (color != null) {
                Debug.DrawRay(position + direction, offset * arrowHeadLength, (Color) color);
            } else {
                Debug.DrawRay(position + direction, offset * arrowHeadLength);
            }
        }
    }

    public static void TextHandle(Vector3 position, string text, Color? color = null, Color? outlineColor = null, int fontSize = 0, float yOffset = 0)
    {
        GUISkin skin = GUI.skin;
        GUIContent textContent = new GUIContent(text);
        GUIStyle style = (skin != null) ? new GUIStyle(skin.GetStyle("Label")) : new GUIStyle();
        Vector2 textSize = Vector2.zero;
        Vector3 screenPoint = Vector3.zero;
        Vector3 worldPosition = Vector3.zero;

        if (outlineColor != null) {
            for (int i = 0; i < 4; i++) {
                Vector2 outlineOffset = Vector2.zero;

                if (i == 0) {
                    outlineOffset = new Vector2(-1f, 0);
                } else if (i == 1) {
                    outlineOffset = new Vector2(1f, 0);
                } else if (i == 2) {
                    outlineOffset = new Vector2(0, -1f);
                } else if (i == 3) {
                    outlineOffset = new Vector2(0, 1f);
                }

                style.normal.textColor = (Color) outlineColor;

                if (fontSize > 0) {
                    style.fontSize = fontSize;
                }

                style.CalcSize(textContent);

                screenPoint = Camera.current.WorldToScreenPoint(position);

                if (screenPoint.z > 0) {
                    worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f + outlineOffset.x, screenPoint.y + textSize.y * 0.5f + yOffset + outlineOffset.y, screenPoint.z));

                    Handles.Label(worldPosition, textContent, style);
                }
            }
        }

        style = (skin != null) ? new GUIStyle(skin.GetStyle("Label")) : new GUIStyle();

        if (color != null) {
            style.normal.textColor = (Color) color;
        }

        if (fontSize > 0) {
            style.fontSize = fontSize;
        }

        style.CalcSize(textContent);

        screenPoint = Camera.current.WorldToScreenPoint(position);

        if (screenPoint.z > 0) {
            worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));

            Handles.Label(worldPosition, textContent, style);
        }
    }
}
