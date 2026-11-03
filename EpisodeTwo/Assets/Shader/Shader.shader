Shader "SolidForeground" {
Properties {
 _Color ("Color", Color) = (1,1,1,0)
}
SubShader { 
 Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Overlay" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  SetTexture [_MainTex] { ConstantColor [_Color] combine constant }
 }
}
}