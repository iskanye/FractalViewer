using Avalonia;
using Avalonia.Controls;
using ComputeSharp;

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
        get => WindowState == WindowState.Maximized ? new((int)Width, (int)Height) : new((int)Width, (int)Height);
    }
    private bool isJulia;
    private Point startPos;

    public MainWindow()
    {
        InitializeComponent();
    }
}