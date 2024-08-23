using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using R3;

namespace AvaloniaApplication1;

public class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
}

public class ViewModel
{
    private int _index = 2;

    private Person[] _possiblePeople =
    {
        new Person { FirstName = "Eleanor", LastName = "Pope", Age = 32 },
        new Person { FirstName = "Jeremy", LastName = "Navarro", Age = 74 },
        new Person { FirstName = "Lailah ", LastName = "Velazquez", Age = 16 },
        new Person { FirstName = "Jazmine", LastName = "Schroeder", Age = 52 },
    };

    private readonly ObservableCollection<Person> _people;

    public ViewModel()
    {
        _people = new();
        _people.Add(_possiblePeople[0]);
        _people.Add(_possiblePeople[1]);
        _people.Add(_possiblePeople[2]);

        Haloo = new("wirld");
        Click = new ReactiveCommand<Unit>();
        Click.Subscribe(_ =>
        {
            var next = _possiblePeople[++_index % _possiblePeople.Length];
            Console.WriteLine("adding: " + next.LastName);
            _people.RemoveAt(0);
            _people.Add(next);
        });

        Source = new FlatTreeDataGridSource<Person>(_people)
        {
            Columns =
            {
                new TextColumn<Person, string>("First Name", x => x.FirstName),
                new TextColumn<Person, string>("Last Name", x => x.LastName),
                new TextColumn<Person, int>("Age", x => x.Age),
            },
        };
    }

    public FlatTreeDataGridSource<Person> Source { get; }
    public ReactiveCommand<Unit> Click { get; }
    public BindableReactiveProperty<string> Haloo { get; }
}
