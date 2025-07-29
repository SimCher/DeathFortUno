// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DoubleVision"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LeftOffset("Left Offset", Range(-1, 0)) = -0.1
		_RightOffset("Right Offset", Range(0, 1)) = 0.1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LeftOffset;
			float _RightOffset;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 leftTexCoord = i.uv + float2(_LeftOffset, 0);
				float2 rightTexCoord = i.uv + float2(_RightOffset, 0);
				
				fixed4 col_l = tex2D(_MainTex, leftTexCoord);
				fixed4 col_r = tex2D(_MainTex, rightTexCoord);

				fixed4 col = lerp(col_l, col_r, 0.5);			
				return col;
			}
			ENDCG
		}
	}
}