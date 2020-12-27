Shader "Custom/HeightStanderd" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ColorScale ("ColorScale", Range(0,1)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #include "./PhotoshopMath.hlsl"

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        half _ColorScale;
        sampler2D _MainTex;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 hsv = float3(frac(IN.worldPos.y * _ColorScale), 1, 1);
            fixed4 c = frac(IN.worldPos.y * 10) < 0.1 ? fixed4(hsv2rgb(hsv), 1) : tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
