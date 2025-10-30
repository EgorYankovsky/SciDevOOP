using SciDevOOP.IO;
using SciDevOOP.IO.Console;
using SciDevOOP.IO.Txt;
using System.IO;

partial class Program
{
    /// <summary>
    /// Method, that reads vector of points (x0, x1, ..., xn, y) from console.
    /// </summary>
    /// <returns>Vectors of points (x0, x1, ..., xn, y).</returns>
    static IList<IList<double>>? Read()
    {
        var reader = new ConsoleReader();
        try
        {
            return reader!.Read();
        }
        catch (InvalidDataException idEx)
        {
            Console.WriteLine($"String {idEx.Message} wasn't in a correct format.\n{idEx}.");
        }
        catch (FormatException fEx)
        {
            Console.WriteLine($"Format exception during {reader!.GetType()} work.\n{fEx.Message}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}.");
        }
        return null;
    }


    /// <summary>
    /// Method, that reads vector of points (x0, x1, ..., xn, y) from file.
    /// </summary>
    /// <param name="path">Path to read data.</param>
    /// <returns>Vectors of points (x0, x1, ..., xn, y).</returns>
    static IList<IList<double>>? Read(string path)
    {
        var extension = string.Empty;
        IReader? reader = null;
        try
        {
            extension = path.Split('\\').Last().Split('.').Last().ToLower();
            reader = extension switch
            {
                "txt" => new TxtReader(path),
                _ => throw new ArgumentException()
            };
            return reader!.Read();
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Can't find file: {ex.Message}.");
        }
        catch (InvalidDataException idEx)
        {
            Console.WriteLine($"String {idEx.Message} wasn't in a correct format.\n{idEx}.");
        }
        catch (FormatException fEx)
        {
            Console.WriteLine($"Format exception during {reader!.GetType()} work.\n{fEx.Message}.");
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Unexpected file extension: {extension}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}.");
        }
        return null;
    }

    /// <summary>
    /// Method, that writes result at console.
    /// </summary>
    /// <param name="answer">Vector of solution.</param>
    static void Write(IList<double> answer)
    {
        var writer = new ConsoleWriter();
        try
        {
            writer!.Write(answer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}.");
        }
    }


    /// <summary>
    /// Method, that writes result at file.
    /// </summary>
    /// <param name="answer">Vector of solution.</param>
    /// <param name="path">Path to write points.</param>
    static void Write(IList<double> answer, string path)
    {
        var extension = string.Empty;
        IWriter? writer = null;
        try
        {
            extension = path.Split('\\').Last().Split('.').Last().ToLower();
            writer = extension switch
            {
                "txt" => new TxtWriter(path),
                _ => throw new ArgumentException()
            };
            writer!.Write(answer);
        }
        catch (NotSupportedException nsEx)
        {
            Console.WriteLine(nsEx.Message);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Can't find file: {ex.Message}.");
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Unexpected file extension: {extension}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}.");
        }
    }
}