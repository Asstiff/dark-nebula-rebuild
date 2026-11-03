Shader "Hidden/DarkNebula/PortalExtensionBlend"
{
 Properties { _MainTex ("Neutral grid",2D)="white" {} _PortalBand ("Original band",Float)=1 _ExtensionBlend ("Portal extension",Float)=0 _Arrived ("At destination",Float)=0 }
 SubShader {
  Tags {"Queue"="Overlay+50" "RenderType"="Transparent"}
  // Composite only the additional camera bands during the portal sequence.
  // View-space coverage stays continuous while the original camera rotates.
  Pass {
   Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;
   float _PortalBand, _ExtensionBlend, _Arrived;
   struct v2f { float4 pos:SV_POSITION; float4 screen:TEXCOORD0; float2 uv:TEXCOORD1; };
   v2f vert(appdata_base v) {
    v2f o; o.pos=UnityObjectToClipPos(v.vertex); o.screen=ComputeScreenPos(o.pos); o.uv=v.texcoord.xy; return o;
   }
   fixed4 frag(v2f i):SV_Target {
    float distance=abs(i.screen.y/i.screen.w-.5)-.5*_PortalBand;
    float signedY=i.screen.y/i.screen.w-.5;
    float previousSide=1-step(0,signedY)*_Arrived;
    float opacity=smoothstep(0,.25*_PortalBand,distance)*_ExtensionBlend*previousSide;
    fixed3 grid=tex2D(_MainTex,i.uv).rgb;
    return fixed4(grid+fixed3(.008,.055,.065),opacity);
   }
   ENDCG
  }
 }
}
