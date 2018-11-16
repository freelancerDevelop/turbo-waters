Shader "Custom/GrimeDraw"
{
    Properties
    {
        _MainTex ("Mask Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        LOD 100

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_nicest

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform sampler2D _MainTex;
            uniform int _CoordinateLength = 0;
            uniform float4 _Coordinate[32];
            uniform float4 _Color[32];
            uniform float _Size[32];

            v2f vert(appdata_t i)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.texcoord;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 c = tex2D(_MainTex, i.uv);

                for (int d = 0; d < _CoordinateLength; d++) {
                    float draw = 1 - saturate(distance(i.uv.xy, _Coordinate[d].xy) * 10 / _Size[d]);

                    c += draw * _Color[d] * 1000000;
                }

                return saturate(c);
            }
            ENDCG
        }
    }
}
