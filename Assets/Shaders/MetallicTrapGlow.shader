Shader "Abyss/MetallicTrapGlow"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Metallic ("Metallic Level", Float) = 0.5
        _Glossiness ("Glossiness", Float) = 0.7
        _GlowColor ("Glow Color", Color) = (1, 0, 0, 1)
        _GlowIntensity ("Glow Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Metallic;
            float _Glossiness;
            fixed4 _GlowColor;
            float _GlowIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Lighting calculations
                float3 lightDir = normalize(float3(0.5, 0.5, -1));
                float3 normal = normalize(i.worldNormal);

                // Metallic shine
                float metallic = pow(max(0, dot(normal, lightDir)), _Glossiness);
                col.rgb += metallic * _Metallic;

                // Heat glow effect
                float glow = sin(_Time.y * 3.0) * 0.5 + 0.5; // Pulsating glow
                glow *= _GlowIntensity;
                col.rgb += _GlowColor.rgb * glow;

                return col;
            }
            ENDCG
        }
    }
}
