using System;

public class ConsoleTest {
    public static void Main(String[] args) {
        int a = 5;
        Console.WriteLine(a);
        a = 6;
        Console.WriteLine(a);

        var myClass = new MyClass() { MyClassId = 5 };
        myClass.MyClassId = 6;

        var mut_myClassExample = new MyClass() { MyClassId = 7 };
        mut_myClassExample.MyClassId = 8;
        ValidlyMutateMyClass(mut_myClassExample);
    }

    static void InvalidlyMutateMyClass(MyClass myClass) {
        myClass.MyClassId++;
    }

    static void ValidlyMutateMyClass(MyClass mut_myClass) {
        mut_myClass.MyClassId = 6;
        mut_myClass.MyClassId++;
    }
}

public class MyClass {
    public int MyClassId { get; set; }
}

