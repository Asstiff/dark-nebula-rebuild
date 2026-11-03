Shader "Hidden/DarkNebula/MenuEdgeExtension"
{
    Properties {
        _MainTex ("Native artwork",2D)="white" {}
        _Backdrop ("Backdrop",Float)=0
        _Edges ("Fade boundaries",Vector)=(-1000000,1000000,40,0)
        _Up ("Camera up",Vector)=(0,1,0,0)
    }
    SubShader {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        Pass {
            Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; float _Backdrop; float4 _Edges,_Up;
            struct appdata { float4 vertex:POSITION; fixed4 color:COLOR; float2 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; fixed4 color:COLOR; float2 uv:TEXCOORD0; float height:TEXCOORD1; };
            v2f vert(appdata v) {
                v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.color=v.color; o.uv=v.uv;
                o.height=dot(mul(unity_ObjectToWorld,v.vertex).xyz,_Up.xyz); return o;
            }
            fixed4 frag(v2f i):SV_Target {
                if (_Backdrop < .5) return fixed4(0,0,0,smoothstep(0.0,1.0,i.color.a));
                fixed4 color=tex2D(_MainTex,i.uv)*i.color;
                float edge=min(i.height-_Edges.x,_Edges.y-i.height);
                color.rgb*=smoothstep(0.0,_Edges.z,edge); color.a=1; return color;
            }
            ENDCG
        }
    }
}
