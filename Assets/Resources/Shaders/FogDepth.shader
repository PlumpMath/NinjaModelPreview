Shader "Fog/FogDepth"
{
	Properties
	{
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
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
				float3 coord : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float4 f4 = mul(unity_ObjectToWorld, v.vertex);
				o.coord = float3(f4.x, f4.y, f4.z);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.0f, 0.0f, 0.0f, 1.0f);
				col.r = i.coord.z / 200.0f + 0.135f;
				col.g = 1.0f - i.coord.y / 10.0f;
				col.b = i.coord.x / 100.0f + 0.5f;
				return col;
			}

			ENDCG
		}
	}
}
