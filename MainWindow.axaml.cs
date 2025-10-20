using Avalonia;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia.Interactivity;

using ComputeSharp;

using System.IO;

namespace FractalViewer;

public partial class MainWindow : Window
{
    private readonly GraphicsDevice gpu = GraphicsDevice.GetDefault();

    private double2 offset;
    private double scale = 300;

    private int Iterations
    {
        get => (int)iterationsSlider.Value;
    }
    private int2 Size
    {
        get
        {
            var size = FrameSize ?? new(0, 0);
            return new((int)size.Width, (int)size.Height);
        }
    }

    private bool isJulia;
    private Point startPos;

    public MainWindow()
    {
        InitializeComponent();
        Render(offset);
        juliaCheckBox.Click += (sender, args) =>
        {
            isJulia = !isJulia;
            Render(offset);
        };
    }

    private void Render(double2 offset)
    {
        using var texture = gpu.AllocateReadWriteTexture2D<Bgra32, float4>(Size.X, Size.Y);
        if (isJulia)
            gpu.For(texture.Width, texture.Height,
                new JuliaShader(texture, new(texture.Width, texture.Height),
                offset, scale, Iterations, new double2(imaginarySlider.Value, .156)));
        else
            gpu.For(texture.Width, texture.Height,
                new MandelbrotShader(texture, new(texture.Width, texture.Height),
                offset, scale, Iterations));

        image.Source = TextureToBitmap(texture);
    }

    private Bitmap TextureToBitmap(ReadWriteTexture2D<Bgra32, Float4> texture)
    {
        texture.Save("temp.png");
        return new Bitmap("temp.png");
    }

    private void Scaling(object sender, PointerWheelEventArgs e)
    {
        scale += e.Delta.Y * scale / 10;
        Render(offset);
    }

    private void DragStart(object sender, PointerPressedEventArgs e) =>
        startPos = e.GetCurrentPoint(this).Position;

    private void Drag(object sender, PointerEventArgs e)
    {
        if (e.Properties.IsLeftButtonPressed)
        {
            var newOffset = (e.GetCurrentPoint(this).Position - startPos) / scale;
            Render(new(offset.X - newOffset.X, offset.Y - newOffset.Y));
        }
    }

    private void DragEnd(object sender, PointerReleasedEventArgs e)
    {
        var newOffset = (e.GetPosition(this) - startPos) / scale;
        offset = new(offset.X - newOffset.X, offset.Y - newOffset.Y);
        Render(offset);
    }

    private void Rerender(object sender, RoutedEventArgs e)
    {
        Render(offset);
    }
}