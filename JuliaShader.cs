using ComputeSharp;

namespace FractalViewer
{
    [ThreadGroupSize(DefaultThreadGroupSizes.XY)]
    [GeneratedComputeShaderDescriptor]
    [RequiresDoublePrecisionSupport]
    public readonly partial struct JuliaShader(
        IReadWriteNormalizedTexture2D<float4> texture,
        int2 size,
        double2 offset,
        double scale,
        int iterations,
        double2 c) : IComputeShader
    {
        public void Execute()
        {   
            double2 z = new((ThreadIds.X - size.X * 0.5f) / scale + offset.X, (ThreadIds.Y - size.Y * 0.5f) / scale + offset.Y);

            // Основной цикл
            for (int i = 0; i < iterations; i++)
            {
                if (z.X * z.X + z.Y * z.Y > 4)
                {
                    texture[ThreadIds.XY].RGBA = Utils.Color(i, iterations, z);
                    return;
                }

                z = new(z.X * z.X - z.Y * z.Y + c.X, 2 * z.X * z.Y + c.Y);
            }

            texture[ThreadIds.XY].RGBA = new(0, 0, 0, 1);
        }
    }
}
