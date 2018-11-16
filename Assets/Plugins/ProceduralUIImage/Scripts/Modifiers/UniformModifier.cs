using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using System;

[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
	[SerializeField] private float radius;

	public float Radius
	{
		get {
			return radius;
		}

		set {
			radius = value;
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius(Rect imageRect)
	{
		return new Vector4(radius, radius, radius, radius);
	}

    public override string ToString()
    {
        return string.Format("({0},{0},{0},{0})", radius);
    }

	#endregion
}
