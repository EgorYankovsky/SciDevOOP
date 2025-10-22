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
using System.Runtime.InteropServices;

/*
 * Read support console and TXT input.
 *  - to use console input use as: Read()
 *  - to use txt input use as: Read("file_name.txt")
 *  
 *  Write support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 */

partial class Program
{
    static void Main(string[] args)
    {
        var points = Read("input.txt");
        var optimizer = new MinimizerMCG();
        var fun = new LineFunctionN();
        var initial = new Vector { 1.0, 1.0 };
        //var minimal = new Vector { 1.0, 1.0 };
        //var maximal = new Vector { 1.0, 1.0 };
        var functional = new L1Norm { points = points };
        var res = optimizer.Minimize(functional, fun, initial);
        //var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }
}