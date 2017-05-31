// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Additive"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
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

			ZWrite Off
			Blend SrcAlpha One

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
			};

			fixed4 _Color;

			v2f vert(appdata_full IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				//#ifdef PIXELSNAP_ON
				//		OUT.vertex = UnityPixelSnap(OUT.vertex);
				//#endif
				return OUT;
			}

			uniform sampler2D _MainTex;

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, IN.texcoord) * IN.color;
				return tex;
			}

			ENDCG
		}
	}
	Fallback "Sprites/Default"
}