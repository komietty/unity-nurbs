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

            float _ColorScale;

            v2f vert (uint vid: SV_VertexID, appdata val) {
                v2f o;
                float3 c = GenGaussianCurvatureColor(vid, _ColorScale);
                o.color = float4(c, 1);
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