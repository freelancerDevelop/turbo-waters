Shader "Custom/Grime"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color2 ("Middle Color", Color) = (1, 1, 1, 1)
        _MainTex2 ("Middle Texture", 2D) = "white" {}
        _Color3 ("Top Color", Color) = (1, 1, 1, 1)
        _MainTex3 ("Top Texture", 2D) = "white" {}
        _GrimeMap ("Grime Map", 2D) = "white" {}
        _NoiseFrequency ("Noise Frequency", Range(0, 50)) = 10
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent-1"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "LightMode" = "ForwardBase"
        }

        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "NoiseSimplex.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            struct appdata_t
            {
                half4 vertex : POSITION;
                half4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half4 color : COLOR0;
                half2 uv : TEXCOORD0;
                half2 uv_2 : TEXCOORD1;
                half2 uv_3 : TEXCOORD2;
                half2 uv_grime : TEXCOORD3;
                half3 world_pos : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };

            uniform half4 _Color;
            uniform sampler2D _MainTex;
            uniform half4 _MainTex_ST;
            uniform half4 _MainTex_TexelSize;
            uniform half4 _Color2;
            uniform sampler2D _MainTex2;
            uniform half4 _MainTex2_ST;
            uniform half4 _MainTex2_TexelSize;
            uniform half4 _Color3;
            uniform sampler2D _MainTex3;
            uniform half4 _MainTex3_ST;
            uniform half4 _MainTex3_TexelSize;
            uniform sampler2D _GrimeMap;
            uniform half4 _GrimeMap_ST;
            uniform half4 _GrimeMap_TexelSize;
            uniform half _NoiseFrequency;

            v2f vert(appdata_t i)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
                o.uv_2 = TRANSFORM_TEX(i.texcoord, _MainTex2);
                o.uv_3 = TRANSFORM_TEX(i.texcoord, _MainTex3);
                o.uv_grime = TRANSFORM_TEX(i.texcoord, _GrimeMap);
                o.world_pos = mul(unity_ObjectToWorld, i.vertex);
                o.color = i.color;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 c = i.color;
                half4 grimeColor = tex2D(_GrimeMap, i.uv_grime);
                half n = 0;

                if (grimeColor.r < 1 || grimeColor.g < 1 || grimeColor.b < 1) {
                    n = snoise(i.world_pos * _NoiseFrequency);
                }

                if (grimeColor.b < 1 && grimeColor.b + n < 1) {
                    c *= tex2D(_MainTex3, i.uv_3) * _Color3;
                    c.a = 1;
                } else if (grimeColor.g < 1 && grimeColor.g + n < 1) {
                    c *= tex2D(_MainTex2, i.uv_2) * _Color2;
                    c.a = 1;
                } else if (grimeColor.r < 1 && grimeColor.r + n < 1) {
                    c *= tex2D(_MainTex, i.uv) * _Color;
                    c.a = 1;
                } else {
                    c.a = 0;
                }

                return c;
            }
            ENDCG
        }
    }
}
