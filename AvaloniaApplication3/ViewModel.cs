using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using ObservableCollections;
using R3;

namespace AvaloniaApplication3;

public class DiskSpaceResult
{
    private readonly DateTimeOffset _time;

    public DiskSpaceResult()
    {
        _time = DateTimeOffset.Now;
    }

    public string Time => _time.ToString("hh:mm:ss");

    public required string BytesUsed { get; init; }
}

public class ViewModel
{

    private readonly ObservableList<DiskSpaceResult> _results;
    public INotifyCollectionChangedSynchronizedView<DiskSpaceResult> ItemsView { get; set; }


    public ViewModel()
    {
        _results = new();
        ItemsView = _results.CreateView(x => x).ToNotifyCollectionChanged();


        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .SelectAwait(async (_, ct) => await FetchDiskUsage(ct))
            .Select(FormatDiskUsage)
            .Select(s => new DiskSpaceResult { BytesUsed = s })
            .Subscribe(next => { _results.Add(next); });

        Haloo = new("wirld");
        Click = new ReactiveCommand<Unit>();
        Click.SubscribeAwait(async (_, ct) =>
        {
            var o = await FetchDiskUsage(ct);
            var r = new DiskSpaceResult { BytesUsed = FormatDiskUsage(o) };
            _results.Add(r);
            Console.WriteLine("adding: " + r.BytesUsed);
        });

        Source = new FlatTreeDataGridSource<DiskSpaceResult>(ItemsView)
        {
            Columns =
            {
                new TextColumn<DiskSpaceResult, string>("Time", x => x.Time),
                new TextColumn<DiskSpaceResult, string>("GiB Used", x => x.BytesUsed),
            },
        };
    }

    private static string FormatDiskUsage(string kib)
    {
        string converted;
        if (int.TryParse(kib, out var n))
        {
            var gib = ((double)n) / 1024 / 1024;
            converted = gib.ToString("N");
        }
        else
        {
            converted = kib;
        }

        return converted;
    }

    private static async Task<string> FetchDiskUsage(CancellationToken ct = default)
    {
        var pinfo = new ProcessStartInfo("ssh", "minion-live df / | tail -1 |  awk '{print $3}'")
        {
            RedirectStandardOutput = true
        };
        using var p = Process.Start(pinfo);
        if (p is null) return "n/a";
        await p.WaitForExitAsync(ct);
        var o = await p.StandardOutput.ReadToEndAsync(ct);
        return o;
    }

    public FlatTreeDataGridSource<DiskSpaceResult> Source { get; }
    public ReactiveCommand<Unit> Click { get; }
    public BindableReactiveProperty<string> Haloo { get; }
}
