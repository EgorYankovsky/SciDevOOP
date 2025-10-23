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
 *  - basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as input folder
 *  - also you can set here a random folder.
 *  
 *  Write support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 *  - basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as output folder
 *  - also you can set here a random folder.
 */

partial class Program
{
    static void Main(string[] args)
    {
        var points = Read("input1.txt");
        var optimizer = new MinimizerMonteCarlo();
        var fun = new LineFunctionN();
        var initial = new Vector { 1.0, 1.0 };
        var minimal = new Vector { -1.0, -1.0 };
        var maximal = new Vector { 3.0, 3.0 };

        var lowerBounds = new List<double> { 0.0 };
        var upperBounds = new List<double> { 5.0 };
        var numberOfPoints = new List<int> { 5 };

        var functional = new IntegrationNorm(lowerBounds, upperBounds, points, numberOfPoints);
        //var functional = new L2Norm{ points = points };

        var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }
}