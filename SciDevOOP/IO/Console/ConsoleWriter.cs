using SciDevOOP.Functions;
using SciDevOOP.ImmutableInterfaces.Functions;

namespace SciDevOOP.IO.Console;

public class ConsoleWriter : IWriter
{
    public void Write(IList<double> values)
    {
        foreach (var value in values)
            System.Console.WriteLine($"{value:0.000000E+00}");
    }

    public void Write(IWritableFunction function)
    {
        System.Console.WriteLine(function.ToString());
    }
}