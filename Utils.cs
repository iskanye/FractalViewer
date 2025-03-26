using System;
using ComputeSharp;

namespace FractalViewer
{
    public static class Utils
    {
        public static float4 Color(float value, float max, double2 z)
        {
            float mod = Hlsl.Sqrt((float)(z.X * z.X + z.Y * z.Y));
            float val = value / max;
            return new float4(
                (mod * val > 1 ? 1 : mod * val),
                0,
                0,
                1
            );
        }
    }
}
