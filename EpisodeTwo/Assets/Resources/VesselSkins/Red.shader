Shader "Special Alpha/(1) Red" {
 Properties { _MainTex ("Base (RGB)", 2D) = "white" {} _Mask ("Mask (Red Channel)", 2D) = "white" {} }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
  LOD 100
  Pass {
   // Use the recovered Episode Two player depth test for portal occlusion.
   ZTest LEqual ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex; sampler2D _Mask; float4 _MainTex_ST;
   struct appdata { float4 vertex : POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
   struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
   v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.color=v.color; o.uv = TRANSFORM_TEX(v.uv, _MainTex); return o; }
   fixed4 frag(v2f i) : SV_Target { fixed4 col = tex2D(_MainTex, i.uv); col.a = tex2D(_Mask, i.uv).r; return col * i.color; }
   ENDCG
  }
 }
}
