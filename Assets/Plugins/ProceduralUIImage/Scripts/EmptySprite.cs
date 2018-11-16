using UnityEngine;
using System.Collections;

namespace UnityEngine.UI.ProceduralImage
{
	public static class EmptySprite
	{
		private static Sprite instance;

		public static Sprite Get()
		{
			if (instance == null) {
				instance = OnePixelWhiteSprite();
			}

			return instance;
		}

		private static Sprite OnePixelWhiteSprite()
		{
			Texture2D tex = new Texture2D(1, 1);

			tex.SetPixel(0, 0, Color.white);
			tex.Apply();

			return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
		}
	}
}
