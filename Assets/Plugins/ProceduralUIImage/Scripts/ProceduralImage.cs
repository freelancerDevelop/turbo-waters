using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

namespace UnityEngine.UI.ProceduralImage
{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Procedural Image")]
	public class ProceduralImage : Image, ICanvasElement
	{
		[SerializeField] private float borderWidth;
		[SerializeField] private Color borderColor = Color.black;
		[SerializeField] private Vector4 innerShadows = Vector4.zero;
		[SerializeField] private Color innerShadowColor = new Color32(0, 0, 0, 60);
		[SerializeField] private float falloffDistance = 1f;
		[SerializeField] private bool isStatic = false;

		private bool isDirty = true;
		private ProceduralImageModifier modifier;
		private Material customMaterial;
		private Vector3[] corners = new Vector3[4];

		public float BorderWidth
		{
			get {
				return borderWidth;
			}

			set {
				borderWidth = value;

				this.MarkAsDirty();
			}
		}

		public Color BorderColor
		{
			get {
				return borderColor;
			}

			set {
				borderColor = value;

				this.MarkAsDirty();
			}
		}

		public float FalloffDistance
		{
			get {
				return falloffDistance;
			}

			set {
				falloffDistance = value;

				this.MarkAsDirty();
			}
		}

		public Vector4 InnerShadows
		{
			get {
				return innerShadows;
			}

			set {
				innerShadows = value;

				this.MarkAsDirty();
			}
		}

		public Color InnerShadowColor
		{
			get {
				return innerShadowColor;
			}

			set {
				innerShadowColor = value;

				this.MarkAsDirty();
			}
		}

		protected ProceduralImageModifier Modifier
		{
			get {
				if (modifier == null) {
					modifier = this.GetComponent<ProceduralImageModifier>();

					if (modifier == null) {
						ModifierType = typeof(FreeModifier);
					}
				}

				return modifier;
			}

			set {
				modifier = value;
			}
		}

		public System.Type ModifierType
		{
			get {
				return Modifier.GetType();
			}

			set {
				if (this.GetComponent<ProceduralImageModifier>() != null) {
					Destroy(this.GetComponent<ProceduralImageModifier>());
				}

				this.gameObject.AddComponent(value);

				Modifier = this.GetComponent<ProceduralImageModifier>();

				this.MarkAsDirty();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			this.sprite = EmptySprite.Get();

			if (this.customMaterial == null) {
				this.customMaterial = new Material(Shader.Find("UI/Procedural UI Image"));
			}

			this.material = this.customMaterial;
		}

		#if UNITY_EDITOR
			protected override void Reset()
			{
				base.Reset();

				this.OnEnable();
			}
		#endif

		public void Update()
		{
			if (!this.isDirty) {
				return;
			}

			this.UpdateMaterial();

			this.isDirty = false;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			this.MarkAsDirty();
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (this.overrideSprite == null) {
				base.OnPopulateMesh(toFill);
				return;
			}

			switch (type) {
				case Type.Simple:
					this.GenerateSimpleSprite(toFill);
					break;
				case Type.Sliced:
					this.GenerateSimpleSprite(toFill);
					break;
				case Type.Tiled:
					this.GenerateSimpleSprite(toFill);
					break;
				case Type.Filled:
					base.OnPopulateMesh(toFill);
					break;
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Rect rect = this.rectTransform.rect;

			this.rectTransform.GetWorldCorners(this.corners);

			float pixelSize = Vector3.Distance(this.corners[1], this.corners[2]) / rect.width;

			pixelSize = pixelSize / this.falloffDistance;

			Vector4 radius = this.FixRadius(Modifier.CalculateRadius(rect));
			Material tempMaterial = base.GetModifiedMaterial(baseMaterial);

			tempMaterial = MaterialHelper.SetMaterialValues(new ProceduralImageMaterialInfo(this.color, rect.width + this.falloffDistance, rect.height + this.falloffDistance, Mathf.Max(pixelSize, 0), radius, Mathf.Max(this.borderWidth, 0), this.borderColor, this.innerShadows, this.innerShadowColor), tempMaterial);

			return tempMaterial;
		}

		private void MarkAsDirty()
		{
			this.SetMaterialDirty();

			this.isDirty = true;
		}

		private void GenerateSimpleSprite(VertexHelper vh)
		{
			Rect r = this.GetPixelAdjustedRect();
			Vector4 v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
			Vector4 uv = new Vector4(0, 0, 1, 1);
			Color color32 = this.color;
			float aa = this.falloffDistance / 2f;

			vh.Clear();
			vh.AddVert(new Vector3(v.x - aa, v.y - aa), color32, new Vector2(uv.x, uv.y));
			vh.AddVert(new Vector3(v.x - aa, v.w + aa), color32, new Vector2(uv.x, uv.w));
			vh.AddVert(new Vector3(v.z + aa, v.w + aa), color32, new Vector2(uv.z, uv.w));
			vh.AddVert(new Vector3(v.z + aa, v.y - aa), color32, new Vector2(uv.z, uv.y));

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		private Vector4 FixRadius(Vector4 vec)
		{
			Rect r = this.rectTransform.rect;

			vec.x = Mathf.Max(vec.x, 0);
			vec.y = Mathf.Max(vec.y, 0);
			vec.z = Mathf.Max(vec.z, 0);
			vec.w = Mathf.Max(vec.w, 0);

			float scaleFactor = Mathf.Min(r.width / (vec.x + vec.y), r.width / (vec.z + vec.w));

			scaleFactor = Mathf.Min(scaleFactor, r.height / (vec.x + vec.w));
			scaleFactor = Mathf.Min(scaleFactor, r.height / (vec.z + vec.y));
			scaleFactor = Mathf.Min(scaleFactor, 1f);

			return vec * scaleFactor;
		}
	}
}
