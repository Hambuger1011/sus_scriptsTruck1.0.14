Shader "Framework/Mobile/Particles/Alpha Blended" 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" { }
		_AlphaTex("Particle Texture", 2D) = "white" {}
	}
	SubShader 
	{ 
		Tags 
		{ 
			"QUEUE"="Transparent" 
			"IGNOREPROJECTOR"="true" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane" 
		}
	 Pass 
	 {
		Tags 
		{ 
		"QUEUE"="Transparent" 
		"IGNOREPROJECTOR"="true" 
		"RenderType"="Transparent" 
		"PreviewType"="Plane" 
		}
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"

		#pragma multi_compile_fog
		#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

		// uniforms
		float4 _MainTex_ST;
		fixed4 _TextureSampleAdd;

		// vertex shader input data
		struct appdata {
		  float3 pos : POSITION;
		  half4 color : COLOR;
		  float3 uv0 : TEXCOORD0;
		  UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		// vertex-to-fragment interpolators
		struct v2f {
		  fixed4 color : COLOR0;
		  float2 uv0 : TEXCOORD0;
		  #if USING_FOG
			fixed fog : TEXCOORD1;
		  #endif
		  float4 pos : SV_POSITION;
		  UNITY_VERTEX_OUTPUT_STEREO
		};

		// vertex shader
		v2f vert (appdata IN) {
		  v2f o;
		  UNITY_SETUP_INSTANCE_ID(IN);
		  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		  half4 color = IN.color;
		  float3 eyePos = mul (UNITY_MATRIX_MV, float4(IN.pos,1)).xyz;
		  half3 viewDir = 0.0;
		  o.color = saturate(color);
		  // compute texture coordinates
		  o.uv0 = IN.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
		  // fog
		  #if USING_FOG
			float fogCoord = length(eyePos.xyz); // radial fog distance
			UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
			o.fog = saturate(unityFogFactor);
		  #endif
		  // transform position
		  o.pos = UnityObjectToClipPos(IN.pos);
		  return o;
		}

		// textures
		sampler2D _MainTex;
		sampler2D _AlphaTex;

		// fragment shader
		fixed4 frag (v2f IN) : SV_Target {
		  fixed4 col = UnityGetUIDiffuseColor(IN.uv0, _MainTex, _AlphaTex, _TextureSampleAdd) * IN.color;
		  return col;
		}

		// texenvs
		//! TexEnv0: 01010103 01010103 [_MainTex]
		ENDCG
		 }
	}
}