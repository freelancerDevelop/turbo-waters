using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.ProceduralImage
{
	public class MaterialHelper
	{
		public static Material SetMaterialValues(ProceduralImageMaterialInfo info, Material baseMaterial)
		{
			if (baseMaterial == null) {
				throw new System.ArgumentNullException("baseMaterial");
			}

			Material m = baseMaterial;

			m.SetColor("_Color", info.color);
			m.SetFloat("_Width", info.width);
			m.SetFloat("_Height", info.height);
			m.SetFloat("_PixelWorldScale", info.pixelWorldScale);
			m.SetVector("_BorderRadius", info.radius);
			m.SetFloat("_BorderWidth", info.borderWidth);
			m.SetColor("_BorderColor", info.borderColor);
			m.SetVector("_InnerShadows", info.innerShadows);
			m.SetColor("_InnerShadowColor", info.innerShadowColor);

			return m;
		}
	}

	public struct ProceduralImageMaterialInfo
	{
		public Color color;
		public float width;
		public float height;
		public float pixelWorldScale;
		public Vector4 radius;
		public float borderWidth;
		public Color borderColor;
		public Vector4 innerShadows;
		public Color innerShadowColor;

		public ProceduralImageMaterialInfo(Color color, float width, float height, float pixelWorldScale, Vector4 radius, float borderWidth, Color borderColor, Vector4 innerShadows, Color innerShadowColor)
		{
			this.color = color;
			this.width = width;
			this.height = height;
			this.pixelWorldScale = pixelWorldScale;
			this.radius = radius;
			this.borderWidth = borderWidth;
			this.borderColor = borderColor;
			this.innerShadows = innerShadows;
			this.innerShadowColor = innerShadowColor;
		}

		public override string ToString()
		{
			return string.Format("c:{0},w:{1},h:{2},pws:{3},radius:{4},bw:{5},bc:{6},is:{7},isc:{8}", color, width, height, pixelWorldScale, radius, borderWidth, borderColor, innerShadows, innerShadowColor);
		}
	}
}
