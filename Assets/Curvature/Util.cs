using UnityEngine;

namespace kmty.gist {
    public static class ComputeShaderUtil {
        public static void Dispatch1D(ComputeShader compute, int kernel, int count) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(count, (int)tx), (int)ty, (int)tz);
        }
        public static void Dispatch2D(ComputeShader compute, int kernel, int width, int height) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(width, (int)tx), GetKernelBlock(height, (int)ty), 1);
        }
        public static void Dispatch3D(ComputeShader compute, int kernel, int width, int height, int depth) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(width, (int)tx), GetKernelBlock(height, (int)ty), GetKernelBlock(depth, (int)tz));
        }
        static int GetKernelBlock(int count, int blockSize) => (count + blockSize - 1) / blockSize;
    }
}
