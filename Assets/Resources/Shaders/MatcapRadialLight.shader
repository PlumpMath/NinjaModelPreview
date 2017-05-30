// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// MatCap Shader, (c) 2015 Jean Moreno

Shader "MatCap/Radial Light"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_HighlightTex("Highlight (RGB)", 2D) = "black" {}
		_Translucency("_Translucency", Range(0.0, 1.0)) = 0.0
		[Toggle(MATCAP_ACCURATE)] _MatCapAccurate("Accurate Calculation", Int) = 0
	}

	Subshader
	{
		Tags{ "RenderType" = "Opaque" }

		Lighting Off
		Fog{ Mode Off }

		Pass
		{
			Tags{ "LightMode" = "Always" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature MATCAP_ACCURATE
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos	: SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_bump : TEXCOORD1;

		#if MATCAP_ACCURATE
				fixed3 tSpace0 : TEXCOORD2;
				fixed3 tSpace1 : TEXCOORD3;
				fixed3 tSpace2 : TEXCOORD4;
		#else
				float3 c0 : TEXCOORD2;
				float3 c1 : TEXCOORD3;
		#endif
				float2 coord : TEXCOORD4;
				float3 vert : TEXCOORD5;
			};

			uniform float4 _MainTex_ST;
			uniform float4 _BumpMap_ST;

			v2f vert(appdata_tan v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_bump = TRANSFORM_TEX(v.texcoord,_BumpMap);

		#if MATCAP_ACCURATE
				//Accurate bump calculation: calculate tangent space matrix and pass it to fragment shader
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
				o.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
				o.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
				o.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);
		#else
				//Faster but less accurate method (especially on non-uniform scaling)
				v.normal = normalize(v.normal);
				v.tangent = normalize(v.tangent);
				TANGENT_SPACE_ROTATION;
				o.c0 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[0].xyz));
				o.c1 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[1].xyz));
		#endif
				o.coord.xy = o.pos.xy;
				float4 pos = mul(unity_ObjectToWorld, v.vertex);
				o.vert.xyz = pos.xyz;
				return o;
			}

			float _Translucency;
			uniform sampler2D _AmbientPalette;
			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform sampler2D _MatCap;
			//uniform sampler2D _FullScreen;
			uniform sampler2D _SolidScreen;
			uniform sampler2D _FogTex;
			uniform float4 _FogColor;
			//uniform sampler2D _NormalScreen;
			//float4 _AmbientLight;
			//float _ScreenRatio;

			fixed4 frag(v2f i) : COLOR
			{
				float2 normals2D;
				float3 baseNorm = float3(0.0f, 0.0f, 1.0f);
				fixed4 tex = tex2D(_MainTex, i.uv);
				//tex *= _AmbientLight;
				fixed3 normals = UnpackNormal(tex2D(_BumpMap, i.uv_bump));
				float tx = abs(i.coord.x) * 0.5f - 0.5f; // abs(i.coord.x)
				float ty = abs(i.coord.y) * 0.5f - 0.5f; // abs(i.coord.y / _ScreenRatio)
				//float range = min(1.0f, max(0.0f, tx * tx + ty * ty - 0.2f));
				float4 mulTex = tex2D(_AmbientPalette, float2(tx, ty));

				float minC = min(tex.r, min(tex.g, tex.b));
				float maxC = max(tex.r, max(tex.g, tex.b));
				float diffC = maxC - minC;

			#if MATCAP_ACCURATE
				//Rotate normals from tangent space to world space
				float3 worldNorm;
				worldNorm.x = dot(i.tSpace0.xyz, normals);
				worldNorm.y = dot(i.tSpace1.xyz, normals);
				worldNorm.z = dot(i.tSpace2.xyz, normals);
				worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
				float4 mc = tex2D(_MatCap, worldNorm.xy * 0.5 + 0.5);
				normals2D = worldNorm.xy;
			#else
				half2 capCoord = half2(dot(i.c0, normals), dot(i.c1, normals));
				float4 mc = tex2D(_MatCap, capCoord*0.5 + 0.5);
				normals2D = capCoord;
			#endif

				tex.rgb = tex.rgb * diffC + tex.rgb * mulTex.rgb * (1.0f - diffC) + pow(tex.rgb - 0.4f, 2.0f);
				//tex.rgb = tex.rgb = lerp(tex.rgb, dot(tex.rgb, float3(0.3, 0.59, 0.11)), range);
				//tex.rgb -= range * 0.3f;

				tex = tex * mc * 2.0;

				//fixed4 normalCaptured = tex2D(_NormalScreen, i.coord);
				fixed4 solid = tex2D(_SolidScreen, i.coord * 0.5f + 0.5f + normals2D * 0.025f);
				fixed4 translucent = pow((solid - 0.05f + max(0.0f, 1.0f - dot(normals, baseNorm) * 2.0f)) * 1.75f, 1.9f);
				tex.rgb = tex.rgb * (1.0f - _Translucency) + translucent.rgb * _Translucency;

				float4 fogTex = tex2D(_FogTex, float2(i.vert.x + _Time.z * 0.033f, i.vert.z * 2.0f + i.vert.y + _Time.z * 0.01f) * 0.05f);
				float4 fogTex2 = tex2D(_FogTex, float2(i.vert.x - _Time.z * 0.02f, i.vert.z * 2.0f + i.vert.y + _Time.z * 0.05f) * 0.05f);
				fogTex.r = max(fogTex.r, fogTex2.r);
				fogTex.g = max(fogTex.g, fogTex2.g);
				fogTex.b = max(fogTex.b, fogTex2.b);
				fogTex.a = max(fogTex.a, fogTex2.a);
				fogTex *= _FogColor;
				fogTex.rgb *= mulTex.rgb;
				fogTex = fogTex * min(1.0f, max(0.0f, 1.1f - i.vert.y * 1.0f)) * fogTex.a;
				tex.rgb = tex.rgb * (1.0f - fogTex.a) + fogTex.rgb * fogTex.a;

				return tex;
			}
			ENDCG
		}
	}

	Fallback "VertexLit"
}