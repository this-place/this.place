// https://forum.unity.com/threads/height-based-gradient-shader.486727/
Shader "Custom/GradientY" {
   Properties{
      _TintYp("Tint Y+", Color) = (1,1,1,1)
      _TintYn("Tint Y-", Color) = (1,1,1,1)
      _Height("Height", Float) = 1
      _MainTex("Albedo (RGB)", 2D) = "white" {}
	  _Color("Main Color"  ,Color)   = (1,1,1,1)
   }
      SubShader{
      Tags{ "RenderType" = "Opaque" "DisableBatching" = "True"}
      LOD 200
 
      CGPROGRAM
      #pragma surface surf Standard fullforwardshadows
      #pragma vertex vert
 
      #pragma target 3.0
 
      struct Input {
         float2 uv_MainTex;
         float dy;
      };
  
      float4 _TintYp;
      float4 _TintYn;
      float _Height;
      sampler2D _MainTex;
	  float4 _Color;
 
      void vert(inout appdata_full v, out Input o) {
         UNITY_INITIALIZE_OUTPUT(Input, o);
 
         float dy = (v.vertex.y) / _Height;
         if (dy < -1) {
            dy = -1;
         } else if (dy > 1) {
            dy = 1;
         }
         o.dy = dy * 0.5 + 0.5;
      }
  
      void surf(Input IN, inout SurfaceOutputStandard o) {
         half4 color;
     
         float4 ty = lerp(_TintYn,_TintYp, IN.dy);
         color = tex2D(_MainTex, IN.uv_MainTex) * ty * _Color;
         o.Albedo = color;
         o.Alpha = color.a;
      }
      ENDCG
   }
   FallBack "Diffuse"
}
