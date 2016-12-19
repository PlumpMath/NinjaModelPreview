// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Fog/FogEffect"
{
	Properties
	{
		_MainTex ("Screen (RGB)", 2D) = "white" {}
		_Depth ("Depth (RGB)", 2D) = "white" {}
		_FogTex("Fog Texture", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Blend ("Intensity", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile INVERT_Y
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _FogTex;
			sampler2D _Depth;
			float4 _MainTex_ST;
			float4 _FogTex_ST;
			float4 _Depth_ST;
			float4 _FogColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float uvY = i.uv.y;
#if defined(INVERT_Y)
				//uvY = 1.0f - uvY;
#endif
				fixed4 depth = tex2D(_Depth, float2(i.uv.x, uvY));
				fixed4 fogColor = tex2D(_FogTex, float2(/*i.uv.x +*/ depth.b * 2.0f, uvY * 1.0f + depth.g * 0.5f + depth.r)) *_FogColor;
				col.a = 1.0f;
				float a = max(0.0f, min(1.0f, pow(depth.g + 0.4f, (depth.r + 0.15f) * 3.0f) * (1.0f - fogColor.a * 0.5f)));
				fogColor.a = 1.0f;
				col.rgb = col.rgb * a + fogColor.rgb * (1.0f - a);
				return col;
			}
			ENDCG
		}
	}
}
