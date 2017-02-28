Shader "Sprites/Radial Light"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Translucency("_Translucency", Range(0.0, 1.0)) = 0.0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		Fog{ Mode Off }

		Pass
		{

			//Tags{ "LightMode" = "Always" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY //PIXELSNAP_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature MATCAP_ACCURATE
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 coord : TEXCOORD1;

				fixed3 tSpace0 : TEXCOORD2;
				fixed3 tSpace1 : TEXCOORD3;
				fixed3 tSpace2 : TEXCOORD4;

				half3 vert : TEXCOORD5;
			};

			fixed4 _Color;

			v2f vert(appdata_full IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;


				fixed3 worldNormal = UnityObjectToWorldNormal(IN.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(IN.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * IN.tangent.w;
				OUT.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
				OUT.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
				OUT.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);

		//#ifdef PIXELSNAP_ON
		//		OUT.vertex = UnityPixelSnap(OUT.vertex);
		//#endif
				OUT.coord.xy = OUT.vertex.xy;

				float4 pos = mul(unity_ObjectToWorld, IN.vertex);
				OUT.vert.xyz = pos.xyz;

				return OUT;
			}

			float _Translucency;
			uniform sampler2D _AmbientPalette;
			uniform sampler2D _MainTex;
			uniform sampler2D _SolidScreen;
			uniform float _EffectAmount;
			uniform sampler2D _FogTex;
			uniform float4 _FogColor;
			//float4 _AmbientLight;
			//float _ScreenRatio;

			fixed4 frag(v2f IN) : COLOR
			{
				half4 tex = tex2D(_MainTex, IN.texcoord) * IN.color;// *_AmbientLight;
				//half tx = abs(IN.coord.x);
				//half ty = abs(IN.coord.y * _ScreenRatio);
				//half range = min(1.0f, max(0.0f, tx * tx + ty * ty - 0.2f));
				
				float tx = abs(IN.coord.x) * 0.5f - 0.5f;
				float ty = abs(IN.coord.y) * 0.5f - 0.5f;
				float4 mulTex = tex2D(_AmbientPalette, float2(tx, ty));

				float minC = min(tex.r, min(tex.g, tex.b));
				float maxC = max(tex.r, max(tex.g, tex.b));
				float diffC = maxC - minC;
				diffC += maxC * 0.3f;
				diffC = min(1.0f, diffC * 2.0f);

				float3 normals = float3(0.0f, 0.0f, 1.0f);
				float3 worldNorm;
				worldNorm.x = dot(IN.tSpace0.xyz, normals);
				worldNorm.y = dot(IN.tSpace1.xyz, normals);
				worldNorm.z = dot(IN.tSpace2.xyz, normals);
				worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
				//float4 mc = tex2D(_MatCap, worldNorm.xy * 0.5 + 0.5);
				float2 normals2D = worldNorm.xy;

				//texcol.rgb = lerp(texcol.rgb, dot(texcol.rgb, float3(0.3, 0.59, 0.11)), range);
				//texcol.rgb -= range * 0.3f;

				tex.rgb = tex.rgb * diffC + tex.rgb * mulTex.rgb * (1.0f - diffC) + pow(tex.rgb - 0.4f, 2.0f);

				fixed4 solid = tex2D(_SolidScreen, IN.coord * 0.5f + 0.5f + normals2D * 0.025f);
				fixed4 translucent = solid + pow(tex * 0.4f + solid * 0.6f - 0.3f, 4.0f) * 2.0f;
				tex.rgb = tex.rgb * (1.0f - _Translucency) + translucent.rgb * _Translucency;


				float4 fogTex = tex2D(_FogTex, float2(IN.vert.x + _Time.z * 0.033f, IN.vert.z * 2.0f + IN.vert.y + _Time.z * 0.01f) * 0.05f);
				float4 fogTex2 = tex2D(_FogTex, float2(IN.vert.x - _Time.z * 0.02f, IN.vert.z * 2.0f + IN.vert.y + _Time.z * 0.05f) * 0.05f);
				fogTex.r = max(fogTex.r, fogTex2.r);
				fogTex.g = max(fogTex.g, fogTex2.g);
				fogTex.b = max(fogTex.b, fogTex2.b);
				fogTex.a = max(fogTex.a, fogTex2.a);
				fogTex *= _FogColor;
				fogTex.rgb *= mulTex.rgb;
				fogTex = fogTex * min(1.0f, max(0.0f, 1.1f - IN.vert.y * 0.5f)) * fogTex.a;
				tex.rgb = tex.rgb * (1.0f - fogTex.a) + fogTex.rgb * fogTex.a;

				return tex;
			}

			ENDCG
		}
	}
	Fallback "Sprites/Default"
}