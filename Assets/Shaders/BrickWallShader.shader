Shader "Abyss/BrickWallShader"
{
    Properties
    {
        _BrickColor ("Brick Color", Color) = (0.8, 0.3, 0.2, 1) // Default brick color (red)
        _MortarColor ("Mortar Color", Color) = (0.6, 0.6, 0.6, 1) // Default mortar color (light gray)
        _TileCount ("Tile Count (Rows)", Float) = 8.0 // Number of rows of bricks
        _BrickLength ("Brick Length", Float) = 0.5 // Length (width) of each brick
        _MortarHeight ("Mortar Height", Float) = 0.05 // Height of mortar lines
        _MainTex ("Base Texture", 2D) = "white" {} // Optional, for added texture
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

            float4 _BrickColor;
            float4 _MortarColor;
            float _TileCount;
            float _BrickLength;
            float _MortarHeight;
            sampler2D _MainTex;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Calculate adjusted brick height based on tile count and mortar height
                float brickHeight = (1.0 - (_TileCount * _MortarHeight)) / _TileCount;
                float brickWidth = _BrickLength; // Directly use the provided brick length

                // Calculate row and column
                float row = floor(uv.y / (brickHeight + _MortarHeight)); // Determine the row by dividing UV y by brick + mortar height
                float column = uv.x / (brickWidth + _MortarHeight); // Determine the column (pre-offset)

                // Determine if row is odd or even to apply offset for staggering
                bool isOddRow = (int(row) % 2 == 1);
                float offset = isOddRow ? 0.5 * brickWidth : 0.0; // Apply half-brick offset to odd rows

                // Calculate position within the brick/mortar unit
                float localX = frac((uv.x + offset) / (brickWidth + _MortarHeight));
                float localY = frac(uv.y / (brickHeight + _MortarHeight));

                // Determine if in brick area or mortar area
                bool isInBrick = (localX * (brickWidth + _MortarHeight) < brickWidth) &&
                                 (localY * (brickHeight + _MortarHeight) < brickHeight);

                // Output color based on whether we're in a brick or mortar
                return isInBrick ? _BrickColor : _MortarColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
