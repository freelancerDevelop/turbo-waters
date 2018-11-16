using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using System;

[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
	[SerializeField] private Vector4 radius;

	public Vector4 Radius
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
		return radius;
	}

	public override string ToString()
	{
		return string.Format("({0},{0},{0},{0})", radius);
	}

	#endregion
}
