Shader "Rhythm Doctor/Wave" {
	Properties {
		_WaveColor ("Wave color", Vector) = (0,1,0,1)
		_WaveHighlightColor ("Wave Highlight color", Vector) = (0,1,0,1)
		_WaveTime ("Wave Times", Vector) = (0,0,0,0)
		_SineHeight ("Sine heights", Vector) = (40,40,40,40)
		_SineWidth ("Sine widths", Vector) = (40,40,40,40)
		_TriangleOffset ("Triangle offsets", Vector) = (0,0,0,0)
		_TriangleOffsetLock ("Triangle offset locks", Vector) = (0,0,0,0)
		_TriangleFrequency ("Triangle frequencies", Vector) = (0.2,0.2,0.2,0.2)
		_CanvasWidth ("Canvas width", Float) = 100
		_CanvasHeight ("Canvas height", Float) = 100
		_Hue ("Hue", Float) = 0
		_ColorOverlay ("Overlay color", Vector) = (0,0,0,0)
		[Enum(Outline,1,Glow,0)] _Outline ("Outline", Float) = 0
		_GlowColor ("Glow/Outline color", Vector) = (0,0,0,0)
		_AdditiveColor ("Additive Color", Vector) = (0,0,0,0)
		_GlowPower ("Glow power", Float) = 1
		_Flash ("Flash", Float) = 0
		_FlashPower ("Flash power", Float) = 2
		_Opacity ("Opacity", Float) = 1
		_PixelScale ("Pixel Art Scale", Float) = 4
		[NoScaleOffset] _OverlayTexture ("Overlay Texture (RGBA)", 2D) = "white" {}
		_OverlayTexturePower ("Overlay Texture Power", Float) = 0
		[NoScaleOffset] _NoiseTexture ("Noise Texture", 2D) = "white" {}
		_BrighterCol ("Fire Color (bright)", Vector) = (1,1,1,1)
		_MiddleCol ("Fire Color (mid)", Vector) = (0.7,0.7,0.7,1)
		_DarkerCol ("Fire Color (dark)", Vector) = (0.4,0.4,0.4,1)
		_NoiseTexturePower ("Noise Texture Power", Float) = 0
		_NoiseSpeed ("Noise Speed", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}