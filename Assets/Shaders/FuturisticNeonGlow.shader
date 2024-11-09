Shader "Abyss/FuturisticNeonGlow"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0, 1, 1, 1) // Neon blue by default
        _GlowIntensity ("Glow Intensity", Float) = 1.0
        _OutlineThickness ("Outline Thickness", Float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            // Vertex Shader
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _OutlineThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                // Sample the main texture
                half4 texColor = tex2D(_MainTex, i.uv);

                // Base color
                half4 color = texColor;

                // Apply glow effect based on the neon color and intensity
                color.rgb += _GlowColor.rgb * _GlowIntensity;

                // Outline detection (detect transparency, edges)
                float2 offset = _OutlineThickness / _ScreenParams.xy;
                half4 edgeColor = tex2D(_MainTex, i.uv + offset);
                if (edgeColor.a < 0.1) // Low alpha (transparent) pixels indicate outline
                {
                    color.rgb = _GlowColor.rgb * _GlowIntensity; // Glow on edges
                }

                return color;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
