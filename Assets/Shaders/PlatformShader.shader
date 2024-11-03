Shader "Abyss/PlatformShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _FillColor("Fill Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Range(0, 0.1)) = 0.05
        _RoundedCorner("Rounded Corner", Range(0, 0.5)) = 0.2
        [Toggle] _LeftEdgeDraw("Left Edge Draw", Float) = 0
        [Toggle] _RightEdgeDraw("Right Edge Draw", Float) = 0
        [Toggle] _TopEdgeDraw("Top Edge Draw", Float) = 0
        [Toggle] _BottomEdgeDraw("Bottom Edge Draw", Float) = 0
        [Toggle] _TopLeftCurved("Top Left Curved", Float) = 0
        [Toggle] _BottomLeftCurved("Bottom Left Curved", Float) = 0
        [Toggle] _TopRightCurved("Top Right Curved", Float) = 0
        [Toggle] _BottomRightCurved("Bottom Right Curved", Float) = 0
        [Toggle] _TopLeftConvexConnector("Top Left Convex Connector", Float) = 0
        [Toggle] _BottomLeftConvexConnector("Bottom Left Convex Connector", Float) = 0
        [Toggle] _TopRightConvexConnector("Top Right Convex Connector", Float) = 0
        [Toggle] _BottomRightConvexConnector("Bottom Right Convex Connector", Float) = 0
        [Toggle] _TopLeftConvexConnectorInverse("Top Left Convex Connector Inverse", Float) = 0
        [Toggle] _BottomLeftConvexConnectorInverse("Bottom Left Convex Connector Inverse", Float) = 0
        [Toggle] _TopRightConvexConnectorInverse("Top Right Convex Connector Inverse", Float) = 0
        [Toggle] _BottomRightConvexConnectorInverse("Bottom Right Convex Connector Inverse", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        Pass
        {
            Name "Default"
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _FillColor;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _RoundedCorner;
            float _LeftEdgeDraw;
            float _RightEdgeDraw;
            float _TopEdgeDraw;
            float _BottomEdgeDraw;
            float _TopLeftCurved;
            float _BottomLeftCurved;
            float _TopRightCurved;
            float _BottomRightCurved;
            float _TopLeftConvexConnector;
            float _BottomLeftConvexConnector;
            float _TopRightConvexConnector;
            float _BottomRightConvexConnector;
            float _TopLeftConvexConnectorInverse;
            float _BottomLeftConvexConnectorInverse;
            float _TopRightConvexConnectorInverse;
            float _BottomRightConvexConnectorInverse;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //Corners will only be curved if both the edges extending from it are drawn
                _TopRightCurved = _TopRightCurved * _RightEdgeDraw * _TopEdgeDraw;
                _TopLeftCurved = _TopLeftCurved * _LeftEdgeDraw * _TopEdgeDraw;
                _BottomRightCurved = _BottomRightCurved * _RightEdgeDraw * _BottomEdgeDraw;
                _BottomLeftCurved = _BottomLeftCurved * _LeftEdgeDraw * _BottomEdgeDraw;

                //Convex connector will only be drawn if both the edges of that corner are NOT drawn
                if(_TopLeftConvexConnector == 1 && (_LeftEdgeDraw != 0 || _TopEdgeDraw != 0)) _TopLeftConvexConnector = 0;
                if(_BottomLeftConvexConnector == 1 && (_LeftEdgeDraw != 0 || _BottomEdgeDraw != 0)) _BottomLeftConvexConnector = 0;
                if(_TopRightConvexConnector == 1 && (_RightEdgeDraw != 0 || _TopEdgeDraw != 0)) _TopRightConvexConnector = 0;
                if(_BottomRightConvexConnector == 1 && (_BottomEdgeDraw != 0 || _RightEdgeDraw != 0)) _BottomRightConvexConnector = 0;
                
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float px = (i.uv.x * 2.0) - 1.0;
                float py = (i.uv.y * 2.0) - 1.0;
                float roundedCornerCenter = 1 - _RoundedCorner;

                float2 bottomLeftRoundedCornerCenter = float2(-roundedCornerCenter, -roundedCornerCenter);
                float2 bottomRightRoundedCornerCenter = float2(roundedCornerCenter, -roundedCornerCenter);
                float2 topLeftRoundedCornerCenter = float2(-roundedCornerCenter, roundedCornerCenter);
                float2 topRightRoundedCornerCenter = float2(roundedCornerCenter, roundedCornerCenter);

                fixed4 finalColor = _FillColor;
                if((px > -roundedCornerCenter && px < roundedCornerCenter) || (py > -roundedCornerCenter && py < roundedCornerCenter))
                {
                    //This if-block is for the (+)Shape formed inside
                    if(px < -1 + _OutlineThickness || px > 1 - _OutlineThickness || py < -1 + _OutlineThickness || py > 1 - _OutlineThickness)
                    {
                        finalColor = _OutlineColor;
                    }
                    if(px < -1 + _OutlineThickness && _LeftEdgeDraw == 0)
                    {
                        finalColor = _FillColor;
                    }
                    else if(px > 1 - _OutlineThickness && _RightEdgeDraw == 0)
                    {
                        finalColor = _FillColor;
                    }
                    else if(py < -1 + _OutlineThickness && _BottomEdgeDraw == 0)
                    {
                        finalColor = _FillColor;
                    }
                    else if(py > 1 - _OutlineThickness && _TopEdgeDraw == 0)
                    {
                        finalColor = _FillColor;
                    }
                }
                else
                {
                    //This else block is for the corners
                    //on the corner segments, how should they be colored?
                    //if draw edge is false, then it just returns the fill color
                    //if rounded edges is turned off, then each edge of this corner segment is evaluated for the outline to be drawn based on the draw outline rule of that edge
                    //if rounded edge is on, then rounded edge algo is evaluated
                    finalColor = float4(0.0, 0.0, 0.0, 0.0);
                    float sqrDist_outlineInnerEdgeToCenter = (_RoundedCorner - _OutlineThickness) * (_RoundedCorner - _OutlineThickness);
                    float sqrDist_outlineOuterEdgeToCenter = _RoundedCorner * _RoundedCorner;
                    if(px < bottomLeftRoundedCornerCenter.x && py < bottomLeftRoundedCornerCenter.y)
                    {
                        if(_BottomLeftCurved != 0)
                        {
                            // float sqrDist_pointToCenter = length(p - bottomLeftRoundedCornerCenter);
                            // sqrDist_pointToCenter = sqrDist_pointToCenter * sqrDist_pointToCenter;
                            float xDist = px - bottomLeftRoundedCornerCenter.x;
                            float yDist = py - bottomLeftRoundedCornerCenter.y;
                            float sqrDist_pointToCenter = (xDist * xDist) + (yDist * yDist);
                            if(sqrDist_pointToCenter < sqrDist_outlineInnerEdgeToCenter)
                            {
                                finalColor = _FillColor;
                            }
                            else if(sqrDist_pointToCenter > sqrDist_outlineInnerEdgeToCenter && sqrDist_pointToCenter < sqrDist_outlineOuterEdgeToCenter)
                            {
                                finalColor = _OutlineColor;
                            }
                        }
                        else
                        {
                            finalColor = _FillColor;
                            if(px < -1 + _OutlineThickness)
                            {
                                if(_LeftEdgeDraw != 0)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(py < -1 + _OutlineThickness)
                            {
                                if(_BottomEdgeDraw != 0 )
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(_BottomLeftConvexConnector == 1 && px < -1 + _OutlineThickness && py < -1 + _OutlineThickness)
                            {
                                float xDist = px - (-1);
                                float yDist = py - (-1);
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            else if(_BottomLeftConvexConnectorInverse == 1 && px < -1 + _OutlineThickness && py < -1 + _OutlineThickness)
                            {
                                float inverseCenterX = -1;
                                float inverseCenterY = -1 + _OutlineThickness;
                                float xDist = px - inverseCenterX;
                                float yDist = py - inverseCenterY;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                        }
                    }
                    else if(px > bottomRightRoundedCornerCenter.x && py < bottomRightRoundedCornerCenter.y)
                    {
                        if(_BottomRightCurved != 0)
                        {
                            // float sqrDist_pointToCenter = length(p - bottomRightRoundedCornerCenter);
                            // sqrDist_pointToCenter = sqrDist_pointToCenter * sqrDist_pointToCenter;
                            float xDist = px - bottomRightRoundedCornerCenter.x;
                            float yDist = py - bottomRightRoundedCornerCenter.y;
                            float sqrDist_pointToCenter = (xDist * xDist) + (yDist * yDist);

                            if(sqrDist_pointToCenter < sqrDist_outlineInnerEdgeToCenter)
                            {
                                finalColor = _FillColor;
                            }
                            else if(sqrDist_pointToCenter > sqrDist_outlineInnerEdgeToCenter && sqrDist_pointToCenter < sqrDist_outlineOuterEdgeToCenter)
                            {
                                finalColor = _OutlineColor;
                            }
                        }
                        else
                        {
                            finalColor = _FillColor;
                            if(px > 1 - _OutlineThickness)
                            {
                                if(_RightEdgeDraw != 0)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(py < -1 + _OutlineThickness)
                            {
                                if(_BottomEdgeDraw != 0 )
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(_BottomRightConvexConnector == 1 && px > 1 - _OutlineThickness && py < -1 + _OutlineThickness)
                            {
                                float xDist = px - 1;
                                float yDist = py - (-1);
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            else if(_BottomRightConvexConnectorInverse == 1 && px > 1 - _OutlineThickness && py < -1 + _OutlineThickness)
                            {
                                float inverseCenterX = 1;
                                float inverseCenterY = -1 + _OutlineThickness;
                                float xDist = px - inverseCenterX;
                                float yDist = py - inverseCenterY;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                        }
                    }
                    else if(px < topLeftRoundedCornerCenter.x && py > topLeftRoundedCornerCenter.y)
                    {
                        if(_TopLeftCurved != 0)
                        {
                            // float sqrDist_pointToCenter = length(p - topLeftRoundedCornerCenter);
                            // sqrDist_pointToCenter = sqrDist_pointToCenter * sqrDist_pointToCenter;
                            float xDist = px - topLeftRoundedCornerCenter.x;
                            float yDist = py - topLeftRoundedCornerCenter.y;
                            float sqrDist_pointToCenter = (xDist * xDist) + (yDist * yDist);

                            if(sqrDist_pointToCenter < sqrDist_outlineInnerEdgeToCenter)
                            {
                                finalColor = _FillColor;
                            }
                            else if(sqrDist_pointToCenter > sqrDist_outlineInnerEdgeToCenter && sqrDist_pointToCenter < sqrDist_outlineOuterEdgeToCenter)
                            {
                                finalColor = _OutlineColor;
                            }
                        }
                        else
                        {
                            finalColor = _FillColor;
                            if(px < -1 + _OutlineThickness)
                            {
                                if(_LeftEdgeDraw != 0)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(py > 1 - _OutlineThickness)
                            {
                                if(_TopEdgeDraw != 0 )
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(_TopLeftConvexConnector == 1 && px < -1 + _OutlineThickness && py > 1 - _OutlineThickness)
                            {
                                float xDist = px - (-1);
                                float yDist = py - 1;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            else if(_TopLeftConvexConnectorInverse == 1 && px < -1 + _OutlineThickness && py > 1 - _OutlineThickness)
                            {
                                float inverseCenterX = -1 + _OutlineThickness;
                                float inverseCenterY = 1;
                                float xDist = px - inverseCenterX;
                                float yDist = py - inverseCenterY;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                        }
                    }
                    else if(px > topRightRoundedCornerCenter.x && py> topRightRoundedCornerCenter.y)
                    {
                        if(_TopRightCurved != 0)
                        {
                            // float sqrDist_pointToCenter = length(p - topRightRoundedCornerCenter);
                            // sqrDist_pointToCenter = sqrDist_pointToCenter * sqrDist_pointToCenter;
                            float xDist = px - topRightRoundedCornerCenter.x;
                            float yDist = py - topRightRoundedCornerCenter.y;
                            float sqrDist_pointToCenter = (xDist * xDist) + (yDist * yDist);

                            if(sqrDist_pointToCenter < sqrDist_outlineInnerEdgeToCenter)
                            {
                                finalColor = _FillColor;
                            }
                            else if(sqrDist_pointToCenter > sqrDist_outlineInnerEdgeToCenter && sqrDist_pointToCenter < sqrDist_outlineOuterEdgeToCenter)
                            {
                                finalColor = _OutlineColor;
                            }
                        }
                        else
                        {
                            finalColor = _FillColor;
                            if(px > 1 - _OutlineThickness)
                            {
                                if(_RightEdgeDraw != 0)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(py > 1 - _OutlineThickness)
                            {
                                if(_TopEdgeDraw != 0 )
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            if(_TopRightConvexConnector == 1 && px >1 - _OutlineThickness && py > 1 - _OutlineThickness)
                            {
                                float xDist = px - 1;
                                float yDist = py - 1;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                            else if(_TopRightConvexConnectorInverse == 1 && px >1 - _OutlineThickness && py > 1 - _OutlineThickness)
                            {
                                float inverseCenterX = 1 - _OutlineThickness;
                                float inverseCenterY = 1;
                                float xDist = px - inverseCenterX;
                                float yDist = py - inverseCenterY;
                                float sqrDist_pointToCorner = (xDist * xDist) + (yDist * yDist);
                                if(sqrDist_pointToCorner < _OutlineThickness * _OutlineThickness)
                                {
                                    finalColor = _OutlineColor;
                                }
                            }
                        }
                    }
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
