Shader "Custom/Gradient Skybox"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 0, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 0, 0)
        _UpVector ("Up Vector", Vector) = (0, 1, 0, 0)
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
			"IgnoreProjector" = "True"
            "Queue" = "Background"
            "RenderType" = "Opaque"
            "PreviewType" = "Skybox"
		}

        ZWrite Off
        Cull Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

			half4 _Color1;
			half4 _Color2;
			half4 _UpVector;
			half _Exponent;
			half _Intensity;

			struct appdata_t
			{
				half4 position : POSITION;
				half3 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half4 position : SV_POSITION;
				half3 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t i)
			{
				v2f o;

				o.position = UnityObjectToClipPos(i.position);
				o.texcoord = i.texcoord;

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				half d = dot(normalize(i.texcoord), half4(0, 1, 0, 0)) * 0.5 + 0.5;

				return lerp(_Color1, _Color2, pow(d, _Exponent)) * _Intensity;
			}
            ENDCG
        }
    }
}