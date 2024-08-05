// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var array = ImmutableArray.Create(1);
        var array2 = array.Add(2);

        var array3 = ImmutableArray<int>.Empty.Add(1);

        Console.WriteLine("abc");
    }
}
