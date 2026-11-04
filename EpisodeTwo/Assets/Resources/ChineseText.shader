Shader "Dark Nebula/Chinese Text"
{
 Properties { _MainTex("Glyph atlas",2D)="white"{} _TintRamp("Recovered font color",2D)="white"{} _StyleRow("Font style",Float)=0.5 _OriginalGradient("Original gradient",2D)="white"{} _UseGradient("Use original gradient",Float)=0 _GradientIndex("Gradient index",Float)=0 _GradientCount("Gradient count",Float)=1 }
 SubShader
 {
  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
  Pass
  {
   Cull Off ZWrite Off ZTest LEqual Blend SrcAlpha OneMinusSrcAlpha
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex, _TintRamp, _OriginalGradient; float _StyleRow, _UseGradient, _GradientIndex, _GradientCount;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;float2 ramp:TEXCOORD1;fixed4 color:COLOR;};
   struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;float2 ramp:TEXCOORD1;fixed4 color:COLOR;};
   v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.ramp=v.ramp;o.color=v.color;return o;}
   fixed4 frag(v2f i):SV_Target
   {
    fixed4 tint=tex2D(_TintRamp,float2(saturate(i.ramp.x),_StyleRow));
    if(_UseGradient>0.5)tint=tex2D(_OriginalGradient,float2((_GradientIndex+i.ramp.y)/_GradientCount,saturate(i.ramp.x)));
    fixed4 color=i.color*tint;
    color.a*=tex2D(_MainTex,i.uv).a;return color;
   }
   ENDCG
  }
 }
}
