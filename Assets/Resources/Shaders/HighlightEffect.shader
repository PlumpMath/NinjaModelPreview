// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Posteffects/HighlightEffect"
{
	Properties
	{
		_MainTex("Screen (RGB)", 2D) = "white" {}
		_BloomTex("Bloom (RGB)", 2D) = "white" {}

		_YOrientation("Y Orientation", Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode Off }
		LOD 200

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
			sampler2D _BloomTex;
			float4 _MainTex_ST;
			float4 _BloomTex_ST;
			float _YOrientation;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float uvY = i.uv.y;
				//#if defined(INVERT_Y)
				uvY = uvY * _YOrientation + (1.0f - uvY) * (1.0f - _YOrientation);
				//#endif
				fixed4 col = tex2D(_MainTex, float2(i.uv.x, uvY));
				fixed4 bloom = tex2D(_BloomTex, float2(i.uv.x, uvY));

				col.r = min(1.0, col.r + bloom.r);
				col.g = min(1.0, col.g + bloom.g);
				col.b = min(1.0, col.b + bloom.b);

				//col = bloom;

				return col;
			}
			ENDCG
		}
	}
}
