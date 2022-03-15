#define PI 3.1415926538
StructuredBuffer<float3> _Vrts;
StructuredBuffer<int3>   _Idxs;
StructuredBuffer<int2>   _Table;

float GenGaussianCurvature(uint vid) {
    float3 v = _Vrts[vid];
    int b = _Table[vid].x;
    int l = _Table[vid].y;
    float s = 0.0;
    for(int i = 0; i < l; i++){
        int frid = _Idxs[i + b].x;
        int t1id = _Idxs[i + b].y;
        int t2id = _Idxs[i + b].z;
        float3 p0 = _Vrts[frid];
        float3 p1 = _Vrts[t1id];
        float3 p2 = _Vrts[t2id];
        float3 d1 = p1 - p0;
        float3 d2 = p2 - p0;
        float l1 = length(d1);
        float l2 = length(d2);
        s += (l1 > 0 && l2 > 0) ? acos(dot(d1/l1, d2/l2)) : 0;
    }
    return (1.0 - s / (PI * 2)) * 12;
}
