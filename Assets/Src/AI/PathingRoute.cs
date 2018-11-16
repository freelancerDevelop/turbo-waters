using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random=UnityEngine.Random;

public class PathingRoute
{
    private PathingMovementType type;
    private Vector3 originPosition;
    private Vector3 targetPosition;

    private Vector3 controlPoint = Vector3.zero;

    public PathingRoute(Vector3 originPosition, Vector3 targetPosition, PathingMovementType type = PathingMovementType.Linear)
    {
        this.originPosition = originPosition;
        this.targetPosition = targetPosition;
        this.type = type;

        this.PreparePath();
    }

    public void PreparePath()
    {
        switch (this.type) {
            case PathingMovementType.QuadraticBezierCurve:
                this.controlPoint = new Vector3(Random.Range(0, 20f), 0, Random.Range(-20f, 0));
                break;
        }
    }

    public Vector3 GetPoint(float ratio)
    {
        switch (this.type) {
            case PathingMovementType.Linear:
                return Vector3.Lerp(this.originPosition, this.targetPosition, ratio);

            case PathingMovementType.QuadraticBezierCurve:
                return Bezier.GetQuadraticBezierCurvePoint(this.originPosition, this.controlPoint, this.targetPosition, ratio);
        }

        return Vector3.zero;
    }
}
