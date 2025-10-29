using SciDevOOP.Functionals;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

partial class Program
{
    static void TestLineN(IOptimizator optimizator, IParametricFunction f)
    {
        var points = Read();
        var initial = new Vector
        {
            0.89,
            0.63
        };
        var functional = new L2Norm()
        {
            points = points
        };
        var res = optimizator.Minimize(functional, f, initial);
        Write(res);
    }

    static void TestPW(IOptimizator optimizator, IParametricFunction f)
    {
        var points = Read("inputPW.txt");
        // Correct answer depends on initial coordinates.
        var initialForMCG = new Vector
        {
            0.7,
            0.4,
            0.1, 0.1, 0.2,
            -1.0, 0.0, 1.0
        };
        var functional = new L2Norm
        {
            points = points
        };
        var res = optimizator.Minimize(functional, f, initialForMCG);
        Write(res);
    }

    static void TestPolynomial(IOptimizator optimizator, IParametricFunction f, IFunctional obj)
    {

    }
    
    static void TestSpline(IOptimizator optimizator, IParametricFunction f)
    {
        var points = Read("inputSpline.txt");
        // Correct answer depends on initial coordinates.
        var initialForMCG = new Vector
        {
            1.5, -1.0117, 1.48, -1.0673, 1.0488, -4.35,
            0.0, 2.0, 5.0
        };
        var functional = new L1Norm
        {
            points = points
        };
        var res = optimizator.Minimize(functional, f, initialForMCG);
        Write(res);
    }
}