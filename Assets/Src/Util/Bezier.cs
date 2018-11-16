using UnityEngine;

public class Bezier
{
    public static Vector3 GetQuadraticBezierCurvePoint(Vector3 control0, Vector3 control1, Vector3 control2, float ratio)
    {
        return Vector3.Lerp(Vector3.Lerp(control0, control1, ratio), Vector3.Lerp(control1, control2, ratio), ratio);
    }

    public static float GetQuadraticBezierCurveLength(Vector3 control0, Vector3 control1, Vector3 control2, int iterations)
    {
        float length = 0;
        float ratio_per_iteration = 1.0f / (float)iterations;
        for (int i = 0; i < iterations - 1; i++) {
            Vector3 start = GetQuadraticBezierCurvePoint(control0, control1, control2, ratio_per_iteration * i);
            Vector3 end = GetQuadraticBezierCurvePoint(control0, control1, control2, ratio_per_iteration * (i + 1));
            length += (start - end).magnitude;
        }
        return length;
    }
}
