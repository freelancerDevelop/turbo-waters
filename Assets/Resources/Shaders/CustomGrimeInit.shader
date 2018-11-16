Shader "Custom/GrimeInit"
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

            struct appdata_t
            {
                half4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            uniform sampler2D _MainTex;
            uniform half4 _MainTex_ST;

            v2f vert(appdata_t i)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(0, 1, 1, 1);
            }
            ENDCG
        }
    }
}
