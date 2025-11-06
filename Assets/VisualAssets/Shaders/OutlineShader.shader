// File: Assets/Shaders/URP_SimpleOutline.shader
Shader "Universal Render Pipeline/Custom/SimpleOutline"
{
    Properties
    {
        _Color ("Outline Color", Color) = (1, 0, 0, 1)
        _Thickness ("Thickness (World Units)", Range(0.0, 0.5)) = 0.06
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry+1" "RenderType"="Opaque" }

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            Cull Front         // draw backfaces to create a shell
            ZWrite Off
            ZTest LEqual       // respect depth; won’t draw through closer geometry

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float  _Thickness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                // expand along **object-space** normal by world-space distance
                // convert thickness to object space by approximating with scale on X (good enough for uniform scales)
                float3x3 M = (float3x3)UNITY_MATRIX_M;
                float approxScale = length(M[0]); // assumes uniform scale; change if needed
                float3 posOS = IN.positionOS.xyz + IN.normalOS * (_Thickness / max(approxScale, 1e-5));
                float4 posWS = mul(UNITY_MATRIX_M, float4(posOS, 1.0));
                OUT.positionHCS = TransformWorldToHClip(posWS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}