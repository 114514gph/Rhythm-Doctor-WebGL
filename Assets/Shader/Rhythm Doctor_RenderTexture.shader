Shader "Rhythm Doctor/RenderTexture" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_MaskTex ("Mask (A)", 2D) = "white" {}
		[Header(Basic Properties)] [Space] _Angle ("Angle", Float) = 0
		_Flash ("Flash", Float) = 0
		_Opacity ("Opacity", Float) = 1
		_Scale ("Scale", Float) = 1
		_WindowScaleX ("WindowScaleX", Float) = 1
		_WindowScaleY ("WindowScaleY", Float) = 1
		_PosX ("PosX", Float) = 0
		_PosY ("PosY", Float) = 0
		_ExtraRatio ("ExtraRatio", Float) = 1
		_BackgroundColor ("Background Color", Vector) = (0,0,0,0)
		_AdditiveColor ("Additive Color", Vector) = (0,0,0,0)
		_ColorOverlay ("Overlay color", Vector) = (0,0,0,0)
		[Header(Perspective Specific)] [Space] [ShowAsVector2] _TopLeft ("Top Left", Vector) = (-0.5,0.5,0,0)
		[ShowAsVector2] _BottomLeft ("Bottom Left", Vector) = (-0.5,-0.5,0,0)
		[ShowAsVector2] _TopRight ("Top Right", Vector) = (0.5,0.5,0,0)
		[ShowAsVector2] _BottomRight ("Bottom Right", Vector) = (0.5,-0.5,0,0)
		_SegmentCount ("Segment Count", Float) = 16
		[Header(Chromakey Specific)] [Space] _KeyColor ("Key Color", Vector) = (0,0,0,0)
		_ColorCutoff ("Cutoff", Range(0, 1)) = 0
		_ColorFeathering ("Color Feathering", Range(0, 1)) = 0.01
		[Header(Toggles)] [Space] _UsingMask ("Using Mask", Float) = 0
		_UsingChromaKey ("Using Chromakey", Float) = 0
		_UsingPerspective ("Using Perspective", Float) = 0
		_InvertMaskAlpha ("Invert Mask Alpha", Float) = 0
		_Premultiply ("Premultiply Alpha", Float) = 1
		[Header(Other)] [Space] _Wobble ("Wobble", Float) = 0
		_WobbleSnap ("Wobble Snap", Float) = 0.2
		[Header(Other)] [Space] _BorderColor ("Border Feedback Color", Vector) = (0,0,0,0)
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