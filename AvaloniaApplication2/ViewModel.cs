using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using R3;

namespace AvaloniaApplication2;

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
    private readonly ObservableCollection<DiskSpaceResult> _results;

    public ViewModel()
    {
        _results = new();

        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .SelectAwait(async (_, ct) => await FetchDiskUsage(ct))
            .Select(FormatDiskUsage)
            .Select(s => new DiskSpaceResult { BytesUsed = s })
            .Subscribe(next => { _results.Insert(0, next); });

        Source = new FlatTreeDataGridSource<DiskSpaceResult>(_results)
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
}
