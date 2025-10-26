using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

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
    static int Main(string[] args)
    {
        var points = Read("input.txt");
        var optimizer = new MinimizerMonteCarlo();
        var fun = new SplineFunction();
        var initial = new Vector { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.0, 2.0, 5.0 };
        TestSpline(initial);
        return 0;
        IVector? minimal = new Vector { 1.5, -1.0117, 1.48, -1.0637, -1.09512, -4.35, -1.0, 1.0, 4.0 };
        IVector? maximal = new Vector { 3.5, 0.9883, 3.48, 0.9327, 1.0488, -2.35, 1.0, 2.0, 6.0 };
        var functional = new MyFunctional { points = points };
        var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }


    static void TestSpline(IVector parameters)
    {
        double x0 = -2.0D;
        double xi = x0;
        double xStep = 0.1D;
        var f = new SplineFunction().Bind(parameters);
        for (int i = 0; xi <= 6.0D; ++i, xi = x0 + i * xStep)
        {
            System.Console.WriteLine($"f({xi}) = {f.Value(new Vector {xi})}");   
        }
    }

}