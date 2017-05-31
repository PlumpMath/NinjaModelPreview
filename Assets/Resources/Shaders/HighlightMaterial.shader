// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Posteffects/HighlightMaterial" {
	Properties {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_HighlightTex("Highglight (RGB)", 2D) = "black" {}
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		//ZTest Always
		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
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

			sampler2D _HighlightTex;
			float4 _HighlightTex_ST;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_HighlightTex, i.uv * _HighlightTex_ST.xy);
				col.rgb *= pow(1.5, 2.0);
				return col;
			}
			ENDCG
		}
	}
}
