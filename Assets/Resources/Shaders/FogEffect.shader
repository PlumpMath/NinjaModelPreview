// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Posteffects/FogEffect"
{
	Properties
	{
		_MainTex ("Screen (RGB)", 2D) = "white" {}
		_Depth ("Depth (RGB)", 2D) = "white" {}
		_FogTex("Fog Texture", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_NearFogColor("Near Fog Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_FarFogColor("Far Fog Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_NearFactor("Near Factor", Range(0.25, 16.0)) = 1
		_FarFactor("Far Factor", Range(0.25, 16.0)) = 1
		_Sharpness("Sharpness", Range(0.25, 16.0)) = 1.0
		_Height("Height", Range(0.0, 2.0)) = 0.0
		_NearHeight("Near Height", Range(0.0, 2.0)) = 0.3
		_FarHeight("Far Height", Range(0.0, 2.0)) = 0.3
		_MiddlePlaneDistance("Middle Plane Distance", Range(0.001, 1.0)) = 0.2
		_MiddlePlaneRange("Middle Plane Range", Range(0.001, 1.0)) = 0.5

		_YOrientation("Y Orientation", Range(0.0, 1.0)) = 1.0

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }
		LOD 200

		Pass
		{
			CGPROGRAM
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
			};

			sampler2D _MainTex;
			sampler2D _Depth;
			sampler2D _FogTex;
			float4 _MainTex_ST;
			float4 _Depth_ST;
			float4 _FogTex_ST;
			float4 _FogColor;
			float4 _NearFogColor;
			float4 _FarFogColor;
			float _NearFactor;
			float _FarFactor;
			float _Sharpness;
			float _Height;
			float _NearHeight;
			float _FarHeight;
			float _MiddlePlaneDistance;
			float _MiddlePlaneRange;
			float _YOrientation;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float uvY = i.uv.y;
//#if defined(INVERT_Y)
				uvY = uvY * _YOrientation + (1.0f - uvY) * (1.0f - _YOrientation);
//#endif
				fixed4 depth = tex2D(_Depth, float2(i.uv.x, uvY));

				fixed middleFactor = min(1.0, max(0.0, (1.0 - abs(depth.r - _MiddlePlaneDistance) * 6.0) * _MiddlePlaneRange));
				fixed nearFactor = pow(min(1.0, max(0.0, 1.0 - depth.r / _MiddlePlaneDistance)), _NearFactor);
				fixed farFactor = pow(min(1.0, max(0.0, depth.r - _MiddlePlaneDistance) / _MiddlePlaneDistance), _FarFactor);
				fixed sumFactor = middleFactor + nearFactor + farFactor;
				fixed factor = (middleFactor + nearFactor + farFactor) / sumFactor;

				fixed4 fogColor = tex2D(_FogTex, float2(depth.b * _FogTex_ST.x, uvY + depth.g * 0.5f + depth.r * _FogTex_ST.y));

				fogColor *= (_FogColor * middleFactor + _NearFogColor * nearFactor + _FarFogColor * farFactor) / sumFactor;

				//fogColor.a = min(1.0f, fogColor.a * 2.0f);

				//col.a = 1.0f;
				fixed height = (_Height * middleFactor + _NearHeight * nearFactor + _FarHeight * farFactor) / sumFactor;
				float a = max(0.0f, min(1.0f, pow(depth.g, _Sharpness) + height)) * fogColor.a;
				fogColor.a = 1.0f;
				col.rgb = col.rgb * (1.0f - a) + fogColor.rgb * a;
				return col;
			}
			ENDCG
		}
	}
}
