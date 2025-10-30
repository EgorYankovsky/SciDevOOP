namespace SciDevOOP.IO.Console;

public class ConsoleWriter : IWriter
{
    public void Write(IList<double> values)
    {
        foreach (var value in values)
            System.Console.WriteLine($"{value:0.000000E+00}");
    }
}