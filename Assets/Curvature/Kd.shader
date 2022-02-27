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
            float _ColorScale;

            v2f vert (uint vid: SV_VertexID, appdata v) {
                v2f o;
                //BgLn bgln = tableBuff[vid];
                //int bg = bgln.bg;
                //int ln = bgln.ln;
                //float sum = 0.0;
                //for(int i = 0; i < ln; i++){
                //    int fr  =  indexBuff[bg + i].fr;
                //    int to1 =  indexBuff[bg + i].to1;
                //    int to2 =  indexBuff[bg + i].to2;
                //    float3 p0 = vertexBuff[fr];
                //    float3 p1 = vertexBuff[to1];
                //    float3 p2 = vertexBuff[to2];
                //    float3 d1 = p1 - p0;
                //    float3 d2 = p2 - p0;
                //    float l1 = length(d1);
                //    float l2 = length(d2);
                //    if(l1 > 0 && l2 > 0){
                //        float3 n1 = d1 / l1;
                //        float3 n2 = d2 / l2;
                //        float rad = acos(dot(n1, n2));
                //        sum += rad;
                //    }
                //}
                //float k = (1.0 - sum) / (PI * 2) * 12 * _ColorScale;

                //o.color = float4(1.0, 0.0, 0.0, 1.0);
                float k = colorBuff[vid] * _ColorScale;
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