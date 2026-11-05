Shader "Custom/SpriteWithSeperateAlpha" {
 Properties { _MainTex ("Base (RGB)", 2D) = "white" {} _Alpha ("Trans (A)", 2D) = "white" {} }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
  LOD 100
  Pass {
   ZTest LEqual ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"

   sampler2D _MainTex; sampler2D _Alpha; float4 _MainTex_ST; float4 _Alpha_ST;
   struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
   struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; float2 alphaUV : TEXCOORD1; };
   v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv, _MainTex); o.alphaUV = TRANSFORM_TEX(v.uv, _Alpha); o.color = v.color; return o; }
   fixed4 frag(v2f i) : SV_Target { fixed4 c = tex2D(_MainTex, i.uv) * i.color; c.a = tex2D(_Alpha, i.alphaUV).a * i.color.a;return c; }
   ENDCG
  }
 }
}
