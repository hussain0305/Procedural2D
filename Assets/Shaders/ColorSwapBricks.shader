Shader "Abyss/ColorSwapBricks"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorFill ("Color Fill (White Replacement)", Color) = (1,1,1,1)
        _ColorLining ("Color Lining (Black Replacement)", Color) = (0,0,0,1)
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.01
        _ReplacementThreshold ("Replacement Threshold", Range(0, 1)) = 0.95
        _BlockSize ("Block Size", Range(0, 0.5)) = 0.01
        _Spacing ("Spacing", Range(0, 0.5)) = 0.01
        _BottomPadding ("Bottom Padding", Range(0, 1)) = 0.1
        _TopPadding ("Top Padding", Range(0, 1)) = 0.9
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
            float _BlockSize;
            float _Spacing;
            float _BottomPadding;
            float _TopPadding;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                #ifdef UNITY_UV_STARTS_AT_TOP
                    o.uv.y = 1.0 - o.uv.y;
                #endif
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);

                // Alpha thresholding
                float alphaBlend = step(_AlphaThreshold, texColor.a);

                // Adjust thresholds for white and black replacement
                float3 whiteThreshold = float3(_ReplacementThreshold, _ReplacementThreshold, _ReplacementThreshold);
                float3 blackThreshold = float3(1.0 - _ReplacementThreshold, 1.0 - _ReplacementThreshold, 1.0 - _ReplacementThreshold);
                
                // Determine if color matches white or black
                float isWhite = step(whiteThreshold, texColor.rgb).r * step(whiteThreshold, texColor.rgb).g * step(whiteThreshold, texColor.rgb).b;
                float isBlack = step(texColor.rgb, blackThreshold).r * step(texColor.rgb, blackThreshold).g * step(texColor.rgb, blackThreshold).b;

                // Select the appropriate color based on white or black detection
                float4 baseColor = lerp(_ColorLining, _ColorFill, isWhite * (1 - isBlack));

                // Vertical padding check
                float inVerticalBounds = step(_BottomPadding, i.uv.y) * step(i.uv.y, 1 - _TopPadding);

                // Block pattern based on UV position
                float totalBlockSize = _BlockSize + _Spacing;
                float2 blockPosition = fmod(i.uv, totalBlockSize);
                float inBlock = step(blockPosition.x, _BlockSize) * step(blockPosition.y, _BlockSize);

                // Combine results for final color
                float4 finalColor = baseColor * alphaBlend * inVerticalBounds * inBlock;
                finalColor.a = finalColor.a * texColor.a; // Preserve texture's alpha for transparency

                return finalColor;
            }
            ENDCG
        }
    }
}
