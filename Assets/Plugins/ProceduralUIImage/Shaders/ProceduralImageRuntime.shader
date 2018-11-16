Shader "UI/Procedural UI Image"
{
	Properties
	{
		[PerRendererData] _MainTex ("Base (RGB)", 2D) = "white" {}
		[PerRendererData] _Color ("Base Color", Color) = (1,1,1,1)

		_Width ("Width", Float) = 100
		_Height ("Height", Float) = 100
		_BorderRadius ("Border Radius", Vector) = (0,0,0,0)
		_BorderWidth ("Border Width", Float) = 0
		_BorderColor ("Border Color", Color) = (0,0,0,0)
		_InnerShadows ("Inner Shadows", Vector) = (0,0,10,0)
		_InnerShadowColor ("Inner Shadow Color", Color) = (0,0,0,0.23)
		_PixelWorldScale ("Pixel World Scale", Float) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			fixed4 _TextureSampleAdd;
			bool _UseClipRect;
			float4 _ClipRect;
			bool _UseAlphaClip;
			half _Width;
			half _Height;
			half _PixelWorldScale;
			half4 _BorderRadius;
			half _BorderWidth;
			sampler2D _MainTex;
			half4 _BorderColor;
			half4 _InnerShadows;
			half4 _InnerShadowColor;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord * float2(_Width,_Height);

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
				#endif

				OUT.color = IN.color*(1+_TextureSampleAdd);

				return OUT;
			}

			half visible(half2 pos, half4 r)
			{
				half4 p = half4(pos, _Width - pos.x, _Height - pos.y);
				half v = min(min(min(p.x, p.y), p.z), p.w);
				bool4 b = bool4(all(p.xw < r[0]), all(p.zw < r[1]), all(p.zy < r[2]), all(p.xy < r[3]));
				half4 vis = r - half4(length(p.xw - r[0]), length(p.zw - r[1]), length(p.zy - r[2]), length(p.xy - r[3]));
				half4 foo = min(b * max(vis, 0), v) + (1 - b) * v;

				v = any(b) * min(min(min(foo.x, foo.y), foo.z), foo.w) + v * (1 - any(b));

				return v;
			}

			half4 frag (v2f IN) : SV_Target
			{
				half4 color = IN.color;

				if (_UseClipRect) {
					color *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				}

				if (_UseAlphaClip) {
					clip(color.a - 0.001);
				}

				half4 shadowColor = lerp(color, _InnerShadowColor, _InnerShadowColor.a);

				if (_InnerShadowColor.a < 1 && color.a == 1) {
					shadowColor.a = color.a;
				}

				if (_InnerShadows.x > 0) {
					half l = (_InnerShadows.x + 1 / _PixelWorldScale) / 2;

					color = lerp(shadowColor, color, saturate(IN.texcoord.x - (_InnerShadows.x + 1 / _PixelWorldScale) / 2));
				}

				if (_InnerShadows.y > 0) {
					half l = (_InnerShadows.y + 1 / _PixelWorldScale) / 2;

					color = lerp(shadowColor, color, saturate((_Height - IN.texcoord.y) - (_InnerShadows.y + 1 / _PixelWorldScale) / 2));
				}

				if (_InnerShadows.z > 0) {
					half l = (_InnerShadows.z + 1 / _PixelWorldScale) / 2;

					color = lerp(shadowColor, color, saturate((_Width - IN.texcoord.x) - (_InnerShadows.z + 1 / _PixelWorldScale) / 2));
				}

				if (_InnerShadows.w > 0) {
					half l = (_InnerShadows.w + 1 / _PixelWorldScale) / 2;

					color = lerp(shadowColor, color, saturate(IN.texcoord.y - (_InnerShadows.w + 1 / _PixelWorldScale) / 2));
				}

				if (_BorderWidth > 0) {
					half l = (_BorderWidth + 1 / _PixelWorldScale) / 2;
					half4 borderColor = lerp(color, _BorderColor, _BorderColor.a);

					color = lerp(borderColor, color, 1 - (saturate(l - distance(visible(IN.texcoord, _BorderRadius), l)) * _PixelWorldScale));
				}

				color.a *= saturate(visible(IN.texcoord, _BorderRadius) * _PixelWorldScale);

				return color;
			}
			ENDCG
		}
	}
}
