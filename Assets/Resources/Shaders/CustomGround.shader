Shader "Custom/Ground"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "" {}

        _OverlayTex ("Overlay Texture", 2D) = "black" {}
        _OverlayColor ("Overlay Color", Color) = (0, 0, 0, 1)
        _OverlayMotionSpeed ("Overlay Motion Speed", Float) = 0
        _OverlayRotation ("Overlay Rotation", Float) = 0
        _OverlayPivotScale ("Overlay Pivot Scale", Vector) = (.5, .5, 1, 1)
        _OverlayTranslation ("Overlay Translation", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                half4 color : COLOR;
                half4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half4 color : COLOR;
                half4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                half2 overlay_uv : TEXCOORD1;
            };

            uniform half4 _Color;
            uniform sampler2D _MainTex;
            uniform half4 _MainTex_ST;
            uniform half4 _BorderColor;
            uniform sampler2D _OverlayTex;
            uniform half4 _OverlayTex_ST;
            uniform half4 _OverlayColor;
            uniform half _OverlayMotionSpeed;
            uniform half _OverlayRotation;
            uniform half4 _OverlayPivotScale;
            uniform half2 _OverlayTranslation;

            v2f vert(appdata_t i)
            {
                v2f o;

                o.color = i.color;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.texcoord = i.texcoord;

                float sinTheta = sin(_OverlayRotation);
                float cosTheta = cos(_OverlayRotation);
                float2x2 rotationMatrix = float2x2(cosTheta, -sinTheta, sinTheta, cosTheta);

                o.overlay_uv = (mul((o.texcoord - _OverlayPivotScale.xy - _OverlayTranslation.xy) * (1 / _OverlayPivotScale.zw), rotationMatrix) + _OverlayPivotScale.xy);
                o.overlay_uv.y -= _Time.x * _OverlayMotionSpeed;

                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                half4 c = tex2D(_MainTex, TRANSFORM_TEX(i.texcoord, _MainTex)) * i.color * _Color;
                half4 overlayC = tex2D(_OverlayTex, TRANSFORM_TEX(i.overlay_uv, _OverlayTex)) * _OverlayColor;

                c += overlayC * overlayC.a * c.a;

                return c;
            }
            ENDCG
        }
    }
}
