Shader "Custom/Toon Shaded"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}

        _ToonLut ("Toon LUT", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "LightMode" = "ForwardBase"
        }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            struct appdata_t
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : TEXCOORD1;
                half3 view_dir : TEXCOORD2;
                half4 color : COLOR;
            };

            uniform sampler2D _MainTex;
            uniform half4 _MainTex_ST;
            uniform sampler2D _ToonLut;
            uniform half3 _RimColor;
            uniform half _RimPower;
            uniform half4 _Color;

            v2f vert(appdata_t v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.view_dir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
                o.color = v.color;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 normal = normalize(i.normal);
                half ndotl = dot(normal, _WorldSpaceLightPos0);
                half ndotv = saturate(dot(normal, i.view_dir));

                half3 lut = tex2D(_ToonLut, float2(ndotl, 0));
                half3 rim = _RimColor * pow(1 - ndotv, _RimPower) * ndotl;

                half3 directDiffuse = lut * _LightColor0;
                half3 indirectDiffuse = unity_AmbientSky;
                half4 c = tex2D(_MainTex, i.uv) * i.color * _Color;

                c.rgb *= directDiffuse + indirectDiffuse;
                c.rgb += rim;

                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
