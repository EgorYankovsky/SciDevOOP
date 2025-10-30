namespace SciDevOOP.IO.Console;

public class ConsoleReader : IReader
{
    public IList<IList<double>>? Read()
    {
        System.Console.WriteLine("Insert values: ");
        IList<IList<double>>? result = null;
        result = [];
        var input = System.Console.ReadLine();
        var isOK = int.TryParse(input, out var pointsAmount);
        if (!isOK) throw new FormatException($"Can't convert {input} to int");
        for (var i = 0; i < pointsAmount; ++i)
        {
            input = System.Console.ReadLine();
            result.Add([.. input.Split(" ").Select(double.Parse)]);
            if (result[i].Count != result.First().Count)
                throw new InvalidDataException(input);
        }
        return result;
    }
}