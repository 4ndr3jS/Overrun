// GOT THIS SHADER FROM THE INTERNET

Shader "Custom/FlashWhite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Flash Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
                float4 color   : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    i.uv
                ).a;

                return float4(
                    _Color.rgb,
                    alpha * _Color.a * i.color.a
                );
            }

            ENDHLSL
        }
    }
}