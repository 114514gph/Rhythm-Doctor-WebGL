Shader "Rhythm Doctor/NewGlow" {
	Properties {
		_MainTex ("Color Texture (RGBA)", 2D) = "white" {}
		[NoScaleOffset] _GlowTex ("Blur Texture (A)", 2D) = "white" {}
		[NoScaleOffset] _OutlineTex ("Outline Texture (A)", 2D) = "white" {}
		[NoScaleOffset] _OverlayTexture ("Overlay Texture (RGBA)", 2D) = "white" {}
		[NoScaleOffset] _PaletteTex ("PaletteTex (RGBA)", 2D) = "white" {}
		_AdditiveColor ("Additive Color", Vector) = (0,0,0,0)
		_ColorOverlay ("Overlay color", Vector) = (0,0,0,0)
		_UsesPalette ("Uses Palette", Float) = 0
		_Flash ("Flash", Float) = 0
		_Opacity ("Opacity", Float) = 1
		_PixelScale ("Pixel Art Scale", Float) = 4
		[Enum(Outline,1,Glow,0)] _Outline ("Outline", Float) = 0
		_GlowColor ("Glow/Outline color", Vector) = (0,0,0,0)
		_GlowPower ("Glow power", Float) = 1
		_HideOutline ("Hide Glow/Outline", Float) = 0
		[NoScaleOffset] _NoiseTexture ("Noise Texture", 2D) = "white" {}
		_BrighterCol ("Fire Color (bright)", Vector) = (1,1,1,1)
		_MiddleCol ("Fire Color (mid)", Vector) = (0.7,0.7,0.7,1)
		_DarkerCol ("Fire Color (dark)", Vector) = (0.4,0.4,0.4,1)
		_UseGlowForNoiseAlpha ("Use Glow for Noise Alpha", Float) = 0
		[Enum(Add,0,Sub,1,ReverseSubtract,2)] _BlendOp ("Blending Operation", Float) = 0
		_BlendFirst ("BlendFirst", Float) = 1
		_BlendSecond ("BlendSecond", Float) = 10
		_OverlayTexturePower ("Overlay Texture Power", Float) = 0
		_NoiseTexturePower ("Noise Texture Power", Float) = 0
		_NoiseSpeed ("Noise Speed", Float) = 1
		_UVBounds ("UVBounds", Vector) = (0,0,1,1)
		_SpecialFade ("Special Fade", Vector) = (0.5,0,0,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
}