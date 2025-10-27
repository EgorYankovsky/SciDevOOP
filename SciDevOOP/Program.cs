using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.Functionals;

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
        var optimizer = new MinimizerMCG();
        var fun = new PiecewiseLinearFunction();
        TestPW(optimizer, fun);
    }

    static void TestLineN(IOptimizator optimizator, IParametricFunction f)
    {

    }

    static void TestPW(IOptimizator optimizator, IParametricFunction f)
    {
        var points = Read("inputPW.txt");
        var initial = new Vector
        {
            0.0,
            0.0,
            0.0, 0.0, 0.0,
            -1.0, 0.0, 1.0
        };
        var functional = new L1Norm
        {
            points = points
        };
        var res = optimizator.Minimize(functional, f, initial);
        Write(res);
    }

    static void TestPolynomial(IOptimizator optimizator, IParametricFunction f, IFunctional obj)
    {

    }
    
    static void TestSpline(IOptimizator optimizator, IParametricFunction f, IFunctional obj)
    {
        
    }
}