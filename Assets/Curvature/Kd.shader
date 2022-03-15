Shader "Unlit/Kd" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "./Curvature.hlsl"
            StructuredBuffer<float> _Curvature;

            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color: COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ColorScale;

            v2f vert (uint vid: SV_VertexID, appdata val) {
                v2f o;
                float k = GenGaussianCurvature(vid) * _ColorScale;
                float e = clamp(k, 0, 1);
                float s = clamp(-k, 0, 1);
                o.color = float4(1 - s, 1 - s - e, 1 - e, 1);
                o.vertex = UnityObjectToClipPos(val.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
}