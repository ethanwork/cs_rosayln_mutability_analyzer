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

        int mut_a = 5;
        Console.WriteLine(mut_a);
        mut_a = 6;
        Console.WriteLine(mut_a);
        int b = 7;
        Console.WriteLine(b);
        b = 8;
        b += 9;
        b++;
        b--;
        --b;
        ++b;
        Console.WriteLine(b);

        MyClass myClass = new MyClass() { A = 5, B = "abc", C = 6, D = "DEF" };
        myClass.A = 7;
        myClass.A += 8;
        Console.WriteLine("myClass.A: " + myClass.A);
        --myClass.A;
        ++myClass.A;
        myClass.A++;
        Console.WriteLine("myClass.A: " + myClass.A);
        myClass.A--;
        myClass.B += "123";

        MyClass mut_myClass2 = new MyClass() { A = 5, B = "abc", C = 6, D = "DEF" };
        mut_myClass2.A = 7;
        mut_myClass2.A += 8;
        mut_myClass2.B += "123";

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(i);
        }

        List<MyClass> myClasses = new ();
        foreach (var item in myClasses)
        {
            Console.WriteLine(item.A);
        }

    }

    static void TestFunction(int a, MyClass b, int mut_c, MyClass mut_d)
    {
        a = 5;
        a += 5;
        a++;
        b.A += 6;
        b.A++;
        --b.A;
        mut_c = 7;
        mut_d.A += 8;
    }
}

public class MyClass
{
    public int A;
    public string B;
    public int C { get; set; }
    public string D { get; set; }
}
