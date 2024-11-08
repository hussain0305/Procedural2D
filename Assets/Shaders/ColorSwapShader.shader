Shader "Abyss/ColorSwapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorFill ("Color Fill (White Replacement)", Color) = (1,1,1,1)
        _ColorLining ("Color Lining (Black Replacement)", Color) = (0,0,0,1)
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.01
        _ReplacementThreshold ("Replacement Threshold", Range(0, 1)) = 0.95
        _UseAbsoluteColors ("Use Absolute Colors", Float) = 0
        _AbsoluteWhite ("Absolute White Color", Color) = (1,1,1,1)
        _AbsoluteBlack ("Absolute Black Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _ColorFill;
            float4 _ColorLining;
            float _AlphaThreshold;
            float _ReplacementThreshold;
            float _UseAbsoluteColors;
            float4 _AbsoluteWhite;
            float4 _AbsoluteBlack;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                if (texColor.a < _AlphaThreshold)
                {
                    return float4(0, 0, 0, 0);
                }
                
                float4 outputColor;

                if (_UseAbsoluteColors > 0.5)
                {
                    if (all(texColor.rgb == _AbsoluteWhite.rgb))
                    {
                        return _ColorFill;
                    }
                    else if (all(texColor.rgb == _AbsoluteBlack.rgb))
                    {
                        return _ColorLining;
                    }
                    else
                    {
                        return float4(0, 0, 0, 0);
                    }
                }
                else
                {
                    if (texColor.r > _ReplacementThreshold && texColor.g > _ReplacementThreshold && texColor.b > _ReplacementThreshold)
                    {
                        return _ColorFill;
                    }
                    else if (texColor.r < 1 - _ReplacementThreshold && texColor.g < 1 - _ReplacementThreshold && texColor.b < 1 - _ReplacementThreshold)
                    {
                        return _ColorLining;
                    }
                    else
                    {
                        return float4(0, 0, 0, 0);
                    }
                }

                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
