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
            #define PI 3.1415926538

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color: COLOR;
            };

            struct FrTo {
                int fr;
                int to1;
                int to2;
            };

            struct BgLn {
                int bg;
                int ln;
            };

            StructuredBuffer<FrTo> indexBuff;
            StructuredBuffer<BgLn> tableBuff;
            StructuredBuffer<float3> vertexBuff;
            StructuredBuffer<float> colorBuff;


            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (uint vid: SV_VertexID, appdata v) {
                v2f o;
                BgLn bgln = tableBuff[vid];
                int bg = bgln.bg;
                int ln = bgln.ln;
                float rad_sum = 0;
                //for(int i = 0; i < ln; i++){
                //    int frid = bg + i;
                //    int fr  =  indexBuff[frid].fr;
                //    int to1 =  indexBuff[frid].to1;
                //    int to2 =  indexBuff[frid].to2;
                //    float3 p0 = vertexBuff[fr];
                //    float3 p1 = vertexBuff[to1];
                //    float3 p2 = vertexBuff[to2];
                //    float3 d1 = p1 - p0;
                //    float3 d2 = p2 - p0;
                //    float3 n1 = d1 / max(length(d1), 1e-5f);
                //    float3 n2 = d2 / max(length(d2), 1e-5f);
                //    float rad = acos(dot(n1, n2));
                //    rad_sum += rad;
                //}

                //o.color = float4(clamp(rad_sum, 0.0, 1.0) / (PI * 2), 0.0, 0.0, 1.0);
                //o.color = float4(1.0, 0.0, 0.0, 1.0);
                float k = colorBuff[vid] * 10;
                o.color = float4(clamp(k, 0, 1), 0, clamp(-k, 0, 1), 1);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
}