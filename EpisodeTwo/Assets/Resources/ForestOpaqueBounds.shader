// Clone-only variant for forest tile backdrops in mixed Matrix scenes.
// Other Mobile/Unlit materials and all entity effects keep their original shader.
Shader "Hidden/DarkNebula/ForestOpaqueBounds"
{
    Properties
    {
        _PortalRegion ("Portal Region",Vector)=(-100000000,100000000,1,1)
        _MainTex ("Base (RGB)",2D)="white" {}
        _BackdropRange ("Backdrop World Z Range",Vector)=(-100000000,100000000,0,0)
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        Pass
        {
            ZWrite On Cull Back Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "PortalRegionClip.cginc"
            sampler2D _MainTex;float4 _MainTex_ST;float4 _BackdropRange;
            struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
            struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;float worldZ:TEXCOORD1;float4 screen:TEXCOORD2;};
            v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=TRANSFORM_TEX(v.uv,_MainTex);o.worldZ=mul(unity_ObjectToWorld,v.vertex).z;o.screen=ComputeScreenPos(o.vertex);return o;}
            fixed4 frag(v2f i):SV_Target {clip(i.worldZ-_BackdropRange.x);clip(_BackdropRange.y-i.worldZ);fixed4 c=tex2D(_MainTex,i.uv);c.a=PortalRegionAlpha(i.worldZ,i.screen);clip(c.a-.0001);return c;}
            ENDCG
        }
    }
}
