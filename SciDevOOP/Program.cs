using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

/*
 * Read support console and TXT input.
 *  - to use console input use as: Read()
 *  - to use txt input use as: Read("file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as input folder
 *      ! also you can set here a random EXISTING! folder.
 *  
 *  Write support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as output folder
 *      ! also you can set here a random EXISTING! folder.
 */

partial class Program
{
    static void Main(string[] args)
    {
        var points = Read("input.txt");
        var optimizer = new MinimizerMCG();
        var fun = new LineFunctionN();
        var initial = new Vector { 0.025, 0.025 };
        var minimal = new Vector { 0.0, 0.0 };
        var maximal = new Vector { 0.45, 0.45 };
        var functional = new L1Norm { points = points };
        //var res = optimizer.Minimize(functional, fun, initial);
        var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }
}