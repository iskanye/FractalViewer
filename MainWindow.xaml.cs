using System;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComputeSharp;

namespace FractalViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            get => WindowState == WindowState.Maximized ? new((int)ActualWidth, (int)ActualHeight) : new((int)Width, (int)Height);
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
            using var texture = gpu.AllocateReadWriteTexture2D<Bgra32, float4>(Size.X, Size.Y - 20);
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

        private BitmapSource TextureToBitmap(ReadWriteTexture2D<Bgra32, Float4> texture)
        {
            PixelFormat pf = PixelFormats.Bgra32;
            int stride = (texture.Width * pf.BitsPerPixel + 7) / 8;

            var pixels = new byte[stride * texture.Height];
            var textureArray = texture.ToArray();
            var index = 0;

            for (int i = 0; i < texture.Height; i++)
            {
                for (int j = 0; j < texture.Width; j++)
                {
                    pixels[index++] = textureArray[i, j].B;
                    pixels[index++] = textureArray[i, j].G;
                    pixels[index++] = textureArray[i, j].R;
                    pixels[index++] = textureArray[i, j].A;
                }
            }

            return BitmapSource.Create(
                texture.Width, texture.Height,
                96, 96, pf, null,
                pixels, stride);
        }

        private void Scaling(object sender, MouseWheelEventArgs e)
        {
            scale += e.Delta * scale / 3000;
            Render(offset);
        }

        private void DragStart(object sender, MouseButtonEventArgs e) =>
            startPos = e.GetPosition(this);

        private void Drag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var newOffset = (e.GetPosition(this) - startPos) / scale;
                Render(new(offset.X - newOffset.X, offset.Y - newOffset.Y));
            }
        }

        private void DragEnd(object sender, MouseButtonEventArgs e)
        {
            var newOffset = (e.GetPosition(this) - startPos) / scale;
            offset = new(offset.X - newOffset.X, offset.Y - newOffset.Y);
            Render(offset);
        }

        private void Rerender(object sender, EventArgs e) =>
            Render(offset);
    }
}