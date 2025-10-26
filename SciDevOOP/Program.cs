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
        var points = Read("input2.txt");
        var optimizer = new MinimizerMonteCarlo();
        var fun = new LineFunctionN();

        // Увеличиваем порядок Гаусса и количество точек
        var initial = new Vector { 1, 1, 1 };
        var minimal = new Vector { 0.0, 0.0, 0.0 };
        var maximal = new Vector { 2.0, 2.0, 2.0 }; // Расширяем границы

        var lowerBounds = new List<double> { 0.0, 0.0 };
        var upperBounds = new List<double> { 1.0, 1.0 }; // Область [0,1]x[0,1]

        // Увеличиваем количество точек Гаусса
        var numberOfPoints = new List<int> { 6, 6 };

        // Используем более высокий порядок Гаусса
        var functional = new IntegrationNorm(lowerBounds, upperBounds, points, 6);

        var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }
}