Shader "Sprites/Radial Light"
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

			//Tags{ "LightMode" = "Always" }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY //PIXELSNAP_ON
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
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
		//#ifdef PIXELSNAP_ON
		//		OUT.vertex = UnityPixelSnap(OUT.vertex);
		//#endif
				OUT.coord.xy = OUT.vertex.xy;
				return OUT;
			}

			uniform sampler2D _AmbientPalette;
			sampler2D _MainTex;
			uniform float _EffectAmount;
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
				float4 mul = tex2D(_AmbientPalette, float2(tx, ty));

				//texcol.rgb = lerp(texcol.rgb, dot(texcol.rgb, float3(0.3, 0.59, 0.11)), range);
				//texcol.rgb -= range * 0.3f;

				tex.rgb = tex.rgb * mul.rgb + pow(tex.rgb - 0.4f, 2.0f);

				return tex;
			}

			ENDCG
		}
	}
	Fallback "Sprites/Default"
}