using System;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

namespace AvaloniaApplication4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContextChanged += (sender, args) =>
        {
            var mw = (MainWindow)sender;
            if (mw is null || mw.DataContext is null) return;
            var vm = (ViewModel)mw.DataContext;
            mw.Tdg.Source = new FlatTreeDataGridSource<DiskSpaceResult>(vm.ItemsView)
            {
                Columns =
                {
                    new TextColumn<DiskSpaceResult, string>("Time", x => x.Time),
                    new TextColumn<DiskSpaceResult, string>("GiB Used", x => x.BytesUsed),
                },
            };
        };
        Tdg.DoubleTapped += (sender, args) =>
        {
            var cellElement = (TextBlock)args.Source!;
            var cellText = cellElement.Text;
            Console.WriteLine($"⚡️ {nameof(Tdg.DoubleTapped)} {cellText}");
        };
    }
}
