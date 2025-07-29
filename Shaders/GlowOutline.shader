Shader "Unlit/GlowOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,0,1)
        _GlowStrength ("Glow Strength", Range(0, 10)) = 2
        _GlowPhase ("Glow Phase", Range(0, 6.28)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "BASE"
            Cull Back
            ZWrite On
            ZTest LEqual
            Lighting Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowStrength;
            float _GlowPhase;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float NdotV = 1 - abs(dot(i.worldNormal, i.viewDir));
                float glowTime = sin(_GlowPhase) * 0.5 + 0.5;
                float glow = pow(NdotV, _GlowStrength * glowTime);
                float4 tex = tex2D(_MainTex, i.uv);
                return tex + _GlowColor * glow;
            }
            ENDCG
        }
    }
Fallback "Unlit/Texture"
}
