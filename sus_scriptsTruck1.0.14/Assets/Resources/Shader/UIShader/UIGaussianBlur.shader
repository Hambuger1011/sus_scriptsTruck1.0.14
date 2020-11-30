Shader "Custom/GaussianBlur"
{
	Properties{
		[PerRendererData]_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BlurRadius("_BlurRadius",Range(1,25)) = 25
	}
		CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	int _BlurRadius;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	float GetGaussianDistribution(float x, float y, float rho) {
		//计算权重
		float g = 1.0f / sqrt(2.0f * 3.141592654f * rho * rho);
		return g * exp(-(x * x + y * y) / (2 * rho * rho));
	}
	float4 GetGaussBlurColor(float2 uv) {
		//参考正态分布曲线图，可以知道 3σ 距离以外的点，权重已经微不足道了
		//反推即可知道当模糊半径为r时，取σ为 r/3 是一个比较合适的取值
		//即真实的模糊半径像素为R/3，映射到0~1为 R*（1/size）/3
		float rho = (float)_BlurRadius * _MainTex_TexelSize.y / 3.0;
		//---权重总和
		float weightTotal = 0;
		for (int x = -_BlurRadius; x <= _BlurRadius; x++) {
			for (int y = -_BlurRadius; y <= _BlurRadius; y++) {
				weightTotal += GetGaussianDistribution(x*_MainTex_TexelSize.y, y *_MainTex_TexelSize.y, rho);
			}
		}
		float4 colorTmp = float4(0, 0, 0, 0);
		for (int x = -_BlurRadius; x <= _BlurRadius; x++) {
			for (int y = -_BlurRadius; y <= _BlurRadius; y++) {
				float weight = GetGaussianDistribution(x * _MainTex_TexelSize.y, y * _MainTex_TexelSize.y, rho) / weightTotal;
				float4 color = tex2D(_MainTex, uv + float2(x * _MainTex_TexelSize.y, y * _MainTex_TexelSize.y));
				color = color * weight;
				colorTmp += color;
			}
		}
		return colorTmp;
	}
	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	fixed4 frag(v2f i) : SV_Target{
		return GetGaussBlurColor(i.uv);
	}
		ENDCG

		SubShader {
		Tags{ "RenderType" = "Opaque" }
			Pass{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			ENDCG
		}
	}
	FallBack off
}
