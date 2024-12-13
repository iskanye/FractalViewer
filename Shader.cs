using ComputeSharp;

namespace FractalViewer
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    [RequiresDoublePrecisionSupport]
    public readonly partial struct Mandelbrot(
        IReadWriteNormalizedTexture2D<float4> texture,
        int2 size,
        double2 offset,
        float scale,
        int iterations) : IComputeShader
    {
        public void Execute()
        {
            double2 z = new();
            double2 c = new((ThreadIds.X - size.X * 0.5f) / scale + offset.X, (ThreadIds.Y - size.Y * 0.5f) / scale + offset.Y);            

            for (int i = 0; i < iterations; i++)
            {
                if (z.X * z.X + z.Y * z.Y > 4)
                {
                    texture[ThreadIds.XY].RGBA = new(i * .01f, 0, 0, 1);
                    return;
                }

                z = new(z.X * z.X - z.Y * z.Y + c.X, 2 * z.X * z.Y + c.Y);
            }

            texture[ThreadIds.XY].RGBA = new(0, 0, 0, 1);
        }
    }
}
