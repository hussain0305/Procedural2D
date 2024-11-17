Shader "Abyss/GlowShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1, 1, 0, 1) // Yellow glow
        _GlowIntensity ("Glow Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha One // Additive blending for glow

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _GlowColor;
            float _GlowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base sprite color
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Add uniform glow
                fixed4 glow = _GlowColor * _GlowIntensity;

                // Combine sprite color and glow
                return texColor + glow;
            }
            ENDCG
        }
    }
}

