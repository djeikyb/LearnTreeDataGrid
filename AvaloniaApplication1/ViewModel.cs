using System;
using R3;

namespace AvaloniaApplication1;

public class ViewModel
{
    public ViewModel()
    {
        Haloo = new("wirld");
        Click = new ReactiveCommand<Unit>();
        Click.Subscribe(_ =>
        {
            var next = _things[++_index % _things.Length];
            Console.WriteLine("click " + next);
            Haloo.Value = next;
        });
    }

    private string[] _things = ["wirld", "oot thar", "… you"];
    private int _index = 0;

    public ReactiveCommand<Unit> Click { get; }
    public BindableReactiveProperty<string> Haloo { get; }
}
