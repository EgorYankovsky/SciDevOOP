using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.IO;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;
using SciDevOOP.IO.Txt;
using SciDevOOP.IO.Console;

var points = Read("input.txt");

var optimizer = new MinimizerMCG();
var fun = new LineFunctionN();
var initial = new Vector { 1.0, 1.0 };

var functional = new L1Norm { points = points };
var res = optimizer.Minimize(functional, fun, initial);
Write(res);

static IList<IList<double>>? Read(string? path = null)
{
    var extension = string.Empty;
    IReader? reader = null;
    try
    {
        if (path is not null)
        {
            extension = path.Split('\\').Last().Split('.').Last().ToLower();
            reader = extension switch
            {
                "txt" => new TxtReader(path),
                _ => throw new ArgumentException()
            };
        }
        else
        {
            reader = new ConsoleReader();
        }
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

static void Write(IList<double> answer, string? path = null)
{
    var extension = string.Empty;
    IWriter? writer = null;
    try
    {
        if (path is not null)
        {
            extension = path.Split('\\').Last().Split('.').Last().ToLower();
            writer = extension switch
            {
                "txt" => new TxtWriter(path),
                _ => throw new ArgumentException()
            };
        }
        else
        {
            writer = new ConsoleWriter();
        }
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
    catch (InvalidDataException idEx)
    {
        Console.WriteLine($"String {idEx.Message} wasn't in a correct format.\n{idEx}.");
    }
    catch (FormatException fEx)
    {
        Console.WriteLine($"Format exception during {writer!.GetType()} work.\n{fEx.Message}.");
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