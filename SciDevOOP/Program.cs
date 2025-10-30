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

partial class Program
{
    static void Main(string[] args)
    {
        // 1. Choose IOptimizator: MinimizerLevenbergMarquardt, MinimizerMCG, MinimizerSimulatedAnnealing.
        IOptimizator optimizer = new MinimizerLevenbergMarquardt();


        // 2. Choose IParametricFunction: LineFunctionN, PiecewiseLinearFunction, Polynomial, SplineFunction.
        var fun = new PiecewiseLinearFunction();
        
        
        // 3. Read points.
        /*
         * Reading support console and TXT input.
         *  - to use console input call as: Read()
         *  - to use txt input call as: Read("file_name.txt")
         *      ! basicly, it reads files from \\SciDevOOP\\bin\\Resources\\ folder
         *      ! also you can set here a full path of EXISTING! file.
        */
        var points = Read("inputPW.txt");


        // 4. Initialize vector of parameters.
        var initial = new Vector
        {
            0.7,
            0.4,
            0.1, 0.1, 0.2,
            -1.0, 0.0, 1.0
        };

        // 4a. If necessary - initialize vector of minimal or maximal parameters.
        var minimal = new Vector
        {
            0.0, -2.0, 0.0, -1.0, -1.0, -4.0,
            0.0, 2.0, 5.0
        };
        var maximal = new Vector
        {
            4.0, 2.0, 4.47, 1.78, 2.88, 0.35,
            0.0, 2.0, 5.0
        };

        // 5. Choose IFunctional: IntegrationNorm, L1Norm, L2Norm, LInfNorm.
        var functional = new L2Norm()
        {
            points = points
        };


        // 6. Solve minimization problem.
        var res = optimizer.Minimize(functional, fun, initial/*, minimal, maximal*/);


        // 7. Write solution.
        /*
         *  Writing support console and TXT output.
         *  - to use console output use as: Write(res)
         *  - to use txt output use as: Read(res, "file_name.txt")
         *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\ as output folder
         *      ! also you can set here a random folder.
        */
        Write(res);
    }
}