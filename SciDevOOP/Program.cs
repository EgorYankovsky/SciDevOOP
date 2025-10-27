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
    static void Main(string[] args)
    {
        var points = Read("inputPW.txt");
        var optimizer = new MinimizerMonteCarlo();
        var fun = new PiecewiseLinearFunction();
        var initial = new Vector
        {
            -0.5,
            0.5,
            0.1, 0.2, 0.3,
            -1.0, 0.0, 1.0
        };
        //TestSpline();
        IVector? minimal = new Vector
        { 
            -1.0,
            -1.0,
            0.0, -2.0, 0.0,
            -1.0, 0.0, 1.0
        };
        IVector? maximal = new Vector
        {
            2.0,
            2.0,
            2.0, 0.0, 2.0,
            -1.0, 0.0, 1.0
        };
        var functional = new MyFunctional
        { 
            points = points 
        };
        var res = optimizer.Minimize(functional, fun, initial, minimal, maximal);
        Write(res);
    }


    static void TestSpline()
    {
        var testParameters = new Vector
        {
            2.423998E+00, 5.027523E-02, 2.459760E+00, 6.372630E-02, 5.835484E-02, -3.137954E+00,
            0.000000E+00, 2.000000E+00, 5.000000E+00
        };

        var xes = new Vector();
        var yes = new Vector();

        double x0 = 0.0D;
        double xi = x0;
        double xStep = 0.1D;
        var f = new SplineFunction().Bind(testParameters);
        for (int i = 0; xi <= 5.0D; ++i, xi = x0 + i * xStep)
        {
            xes.Add(xi);
            yes.Add(f.Value(new Vector { xi }));
        }
        foreach (var x in xes) Console.WriteLine(x);
        Console.WriteLine();  
        foreach (var y in yes) Console.WriteLine(y);   
    }
}