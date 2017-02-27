// MatCap Shader, (c) 2015 Jean Moreno

Shader "Particles/Radial Light"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		//_TintColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	Subshader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Off
		Lighting Off
		Fog{ Mode Off }

		Pass
		{
			//Tags{ "LightMode" = "Always" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature MATCAP_ACCURATE
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos	: SV_POSITION;
				float4 col	: COLOR;
				float2 uv : TEXCOORD0;
				float2 uv_bump : TEXCOORD1;
				float2 coord : TEXCOORD4;
				float3 vert : TEXCOORD5;
			};

			uniform float4 _MainTex_ST;
			uniform float4 _BumpMap_ST;
			//float4 _TintColor;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.col = v.color;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_bump = TRANSFORM_TEX(v.texcoord,_BumpMap);
				o.coord.xy = o.pos.xy;
				float4 pos = mul(unity_ObjectToWorld, v.vertex);
				o.vert.xyz = pos.xyz;
				return o;
			}

			uniform sampler2D _AmbientPalette;
			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform sampler2D _MatCap;
			uniform sampler2D _FogTex;
			uniform float4 _FogColor;
			//float4 _AmbientLight;
			//float _ScreenRatio;

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				//tex *= _AmbientLight;
				//float tx = abs(i.coord.x);
				//float ty = abs(i.coord.y * _ScreenRatio);
				//float range = min(1.0f, max(0.0f, tx * tx + ty * ty - 0.2f));
				float tx = abs(i.coord.x) * 0.5f - 0.5f;
				float ty = abs(i.coord.y) * 0.5f - 0.5f;
				float4 mulTex = tex2D(_AmbientPalette, float2(tx, ty));

				//tex.rgb = tex.rgb = lerp(tex.rgb, dot(tex.rgb, float3(0.3, 0.59, 0.11)), range);
				//tex.rgb -= range * 0.3f;

				tex.rgb = tex.rgb * mulTex.rgb + pow(tex.rgb - 0.4f, 2.0f);

				float4 fogTex = tex2D(_FogTex, float2(i.vert.x + _Time.z * 0.033f, i.vert.z * 2.0f + i.vert.y + _Time.z * 0.01f) * 0.05f);
				float4 fogTex2 = tex2D(_FogTex, float2(i.vert.x - _Time.z * 0.02f, i.vert.z * 2.0f + i.vert.y + _Time.z * 0.05f) * 0.05f);
				fogTex.r = max(fogTex.r, fogTex2.r);
				fogTex.g = max(fogTex.g, fogTex2.g);
				fogTex.b = max(fogTex.b, fogTex2.b);
				fogTex.a = max(fogTex.a, fogTex2.a);
				fogTex *= _FogColor;
				fogTex.rgb *= mulTex.rgb;
				fogTex = fogTex * min(1.0f, max(0.0f, 1.1f - i.vert.y * 0.5f)) * fogTex.a;
				tex.rgb = tex.rgb * (1.0f - fogTex.a) + fogTex.rgb * fogTex.a;

				return tex * i.col;
			}
			ENDCG
		}
	}

	Fallback "VertexLit"
}