Shader "Hidden/DarkNebula/PortalNeutralBacking"
{
 Properties { _MainTex ("Neutral grid",2D)="white" {} _ForestEnd ("Forest end",Float)=0 _PortalBand ("Original band",Float)=1 _PortalAlpha ("Opacity",Float)=0 }
 SubShader {
  Tags {"Queue"="Geometry+501" "RenderType"="Transparent"}
  Pass {
   Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;float _ForestEnd,_PortalBand,_PortalAlpha;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
   struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;float worldZ:TEXCOORD1;float4 screen:TEXCOORD2;};
   v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.worldZ=mul(unity_ObjectToWorld,v.vertex).z;o.screen=ComputeScreenPos(o.vertex);return o;}
   fixed4 frag(v2f i):SV_Target {
    float outside=step(.5*_PortalBand,abs(i.screen.y/i.screen.w-.5));
    // Keep the original forest inside the original view. The added bands use
    // abstract backing after that forest has left the current portal region.
    float fade=saturate((i.worldZ-_ForestEnd)/32);
    fixed4 c=tex2D(_MainTex,i.uv);c.a*=_PortalAlpha*max(outside,fade);clip(c.a-.0001);return c;
   }
   ENDCG
  }
 }
}
