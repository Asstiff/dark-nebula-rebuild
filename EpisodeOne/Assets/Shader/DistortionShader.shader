Shader "Custom/DistortionShader" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _RandomTex ("Random Source", 2D) = "gray" {}
 _Distortion ("_Distortion", Float) = 5
 _Random ("_Random", Float) = 0.5
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}