using System;
using Avalonia.Controls;

namespace AvaloniaApplication3;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Tdg.DoubleTapped += (sender, args) =>
        {
            var cellElement = (TextBlock)args.Source!;
            var cellText = cellElement.Text;
            Console.WriteLine($"⚡️ {nameof(Tdg.DoubleTapped)} {cellText}");
        };
    }
}
