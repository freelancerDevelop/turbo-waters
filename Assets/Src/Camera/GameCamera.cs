using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 initialOffset;

    [Header("Lens Options")]
    public float zoomRatio = 1f;

    public void LateUpdate()
    {
        float screenRatio = (float) Screen.width / (float) Screen.height;

        if (this.target == null) {
            return;
        }

        Vector3 targetPosition = new Vector3(this.target.transform.position.x, 0, this.target.transform.position.z);
        float zoomScaleRatio = 1f + 0.5f * (1f / Mathf.Min(1f, screenRatio) - 1f);

        this.transform.position = targetPosition + this.initialOffset * this.zoomRatio * zoomScaleRatio;

        this.transform.LookAt(targetPosition);
    }
}
