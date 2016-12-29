// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Posteffects/CopyScreenEffect"
{
	Properties
	{
		_MainTex("Screen (RGB)", 2D) = "white" {}
		_YOrientation("Y Orientation", Range(0.0, 1.0)) = 1.0
		_SpreadX("Spread X", Range(0.001, 1.0)) = 0.01
		_SpreadY("Spread Y", Range(0.001, 1.0)) = 0.01
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
			//#pragma target 3.0
			//#pragma glsl
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile INVERT_Y

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
				float2 uv_MainTex : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _YOrientation;
			float _SpreadX;
			float _SpreadY;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float uvY = i.uv.y;
				//#if defined(INVERT_Y)
				uvY = uvY * _YOrientation + (1.0f - uvY) * (1.0f - _YOrientation);
				//#endif
				//fixed4 col = tex2Dlod(_MainTex, float4(i.uv.x, uvY, 0.0, _LOD));
				float lodX = _SpreadX;
				float lodY = _SpreadY;
				//float uvdx = clamp(ddx(i.uv) * 100.0, -1.0, 1.0) * lod;
				//float uvdy = clamp(ddy(i.uv) * 100.0, -1.0, 1.0) * lod;
				//fixed4 col1 = tex2D(_MainTex, float2(i.uv.x, uvY), float2(-uvdx * 10.0, -uvdx * 30.0), float2(uvdx * 10.0, uvdx * 30.0));
				fixed4 col0 = tex2D(_MainTex, float2(i.uv.x, uvY));
				fixed4 col1 = tex2D(_MainTex, float2(i.uv.x, uvY));
				fixed4 col2 = tex2D(_MainTex, float2(i.uv.x - lodX * 0.7f, uvY + lodY * 0.7f));
				fixed4 col3 = tex2D(_MainTex, float2(i.uv.x + lodX * 0.7f, uvY + lodY * 0.7f));
				fixed4 col4 = tex2D(_MainTex, float2(i.uv.x, uvY + lodY * 2.0f));
				fixed4 col5 = tex2D(_MainTex, float2(i.uv.x + lodX * 1.4f, uvY - lodY * 1.4f));
				fixed4 col6 = tex2D(_MainTex, float2(i.uv.x - lodX * 1.4f, uvY - lodY * 1.4f));
				fixed4 col7 = tex2D(_MainTex, float2(i.uv.x, uvY - lodY * 1.5f));
				fixed4 col8 = tex2D(_MainTex, float2(i.uv.x - lodX * 0.7f * 1.5f, uvY + lodY * 0.7f * 1.5f));
				fixed4 col9 = tex2D(_MainTex, float2(i.uv.x + lodX * 0.7f * 1.5f, uvY + lodY * 0.7f * 1.5f));
				fixed4 col10 = tex2D(_MainTex, float2(i.uv.x, uvY + lodY * 2.0f * 1.5f));
				fixed4 col11 = tex2D(_MainTex, float2(i.uv.x + lodX * 1.4f * 1.5f, uvY - lodY * 1.4f * 1.5f));
				fixed4 col12 = tex2D(_MainTex, float2(i.uv.x - lodX * 1.4f * 1.5f, uvY - lodY * 1.4f * 1.5f));
				//fixed4 col2 = tex2D(_MainTex, float2(i.uv.x, uvY), float2(-uvdx * 100.0, uvdy * 200.0), float2(uvdx * 100.0, -uvdy * 300.0));
				//fixed4 col3 = tex2D(_MainTex, float2(i.uv.x, uvY), float2(uvdx * 0.0, -uvdy * 200.0), float2(uvdx * 0.0, uvdy * 300.0));
				//col.b = 0.0f;
				//col2.g = 0.0f;
				//col.r = clamp(ddx(i.uv) * 100.0, 0.0, 1.0);
				//col.g = clamp(ddy(i.uv) * 100.0, 0.0, 1.0);
				//col.b = 0.0f;
				//col = 100.0 * half4(ddx(i.uv_MainTex.x), -ddx(i.uv_MainTex.x), ddx(i.uv_MainTex.y), 1.0);
				//col.r = ddx(i.uv_MainTex.x) * 100.0;
				//col.g = ddy(i.uv_MainTex.y) * 100.0;
				//col.b = 0.0;
				fixed4 col = (col0 + col1 + col2 + col3 + col4 + col5 + col6 + col7 + col8 + col9 + col10 + col11 + col12) / 13.0;
				//col.r = pow(col.r + 0.6, 2.0) - 0.6;
				//col.g = pow(col.g + 0.6, 2.0) - 0.6;
				//col.b = pow(col.b + 0.6, 2.0) - 0.6;
				return col;
			}
			ENDCG
		}
	}
}
