Shader "tk2d/SolidVertexColor" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="Vertex" "IGNOREPROJECTOR"="True" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "IGNOREPROJECTOR"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Cull Off
  SetTexture [_MainTex] { combine texture * primary }
 }
}
}