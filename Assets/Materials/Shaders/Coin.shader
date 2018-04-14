// https://forum.unity.com/threads/height-based-gradient-shader.486727/
Shader "Custom/Coin" {
   Properties {
      _TintXp ("Tint X+", Color) = (1,1,1,1)
      _TintXn ("Tint X-", Color) = (1,1,1,1)
      _Height ("Height", Float) = 1
      _MainTex ("Albedo (RGB)", 2D) = "white" {}
	  _Color ("Main Color"  ,Color)   = (1,1,1,1)
	  _Metallic ("Metallic", Range(0,1)) = 0.0
	  _Smoothness ("Smoothness", Range(0,1)) = 0.0
   }
      SubShader {
      Tags { "RenderType" = "Opaque" "DisableBatching" = "True" }
      LOD 200
 
      CGPROGRAM
      #pragma surface surf Standard fullforwardshadows
      #pragma vertex vert
 
      #pragma target 3.0
 
      struct Input {
         float2 uv_MainTex;
         float dx;
      };
  
      float4 _TintXp;
      float4 _TintXn;
      float _Height;
      sampler2D _MainTex;
	  float4 _Color;
	  half _Metallic;
	  half _Smoothness;
 
      void vert(inout appdata_full v, out Input o) {
         UNITY_INITIALIZE_OUTPUT(Input, o);
 
         float dx = (v.vertex.x) / _Height;
         if (dx < -1) {
            dx = -1;
         } else if (dx > 1) {
            dx = 1;
         }
         o.dx = dx * 0.5 + 0.5;
      }
  
      void surf(Input IN, inout SurfaceOutputStandard o) {
         half4 color;
     
         float4 tx = lerp(_TintXn,_TintXp, IN.dx);
         color = tex2D(_MainTex, IN.uv_MainTex) * tx * _Color;
         o.Albedo = color;
         o.Alpha = color.a;
		 o.Metallic = _Metallic;
		 o.Smoothness = _Smoothness;
      }
      ENDCG
   }
   FallBack "Diffuse"
}
