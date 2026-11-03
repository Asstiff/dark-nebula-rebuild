Shader "FLD/AlphaMapGreen" {
 Properties { _PortalRegion ("Portal Region",Vector)=(-100000000,100000000,1,1) _MainTex ("Base (RGB)", 2D) = "white" {} _Mask ("Mask (Green Channel)", 2D) = "white" {} _BackdropRange ("Backdrop World Z Range", Vector) = (-100000000,100000000,0,0) }
 SubShader {
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
  LOD 100
  Pass {
   ZTest Always ZWrite On Cull Off Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"

   sampler2D _MainTex; sampler2D _Mask; float4 _MainTex_ST; float4 _BackdropRange;
   struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
   struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; float worldZ : TEXCOORD1; float4 screen : TEXCOORD2; };
   v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv, _MainTex); o.worldZ=mul(unity_ObjectToWorld,v.vertex).z; o.screen=ComputeScreenPos(o.vertex); return o; }
   fixed4 frag(v2f i) : SV_Target { fixed4 col = tex2D(_MainTex, i.uv); col.a = tex2D(_Mask, i.uv).g; return col; }
   ENDCG
  }
 }
}
