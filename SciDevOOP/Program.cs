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
 * Reading support console and TXT input.
 *  - to use console input call as: Read()
 *  - to use txt input call as: Read("file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\ as input folder
 *      ! also you can set here a full path of EXISTING! file.
 *  
 *  Writing support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\ as output folder
 *      ! also you can set here a random folder.
 */

partial class Program
{
    static void Main(string[] args)
    {
        IOptimizator optimizer = new MinimizerLevenbergMarquardt();
        var fun = new PiecewiseLinearFunction();
        var points = Read("inputPW.txt");
        var initial = new Vector
        {
            0.7,
            0.4,
            0.1, 0.1, 0.2,
            -1.0, 0.0, 1.0
        };
        //var minimal = new Vector
        //{
        //    0.0, -2.0, 0.0, -1.0, -1.0, -4.0,
        //    0.0, 2.0, 5.0
        //};
        //var maximal = new Vector
        //{
        //    4.0, 2.0, 4.47, 1.78, 2.88, 0.35,
        //    0.0, 2.0, 5.0
        //};

        var functional = new L2Norm()
        {
            points = points
        };
        var res = optimizer.Minimize(functional, fun, initial/*, minimal, maximal*/);
        Write(res);
    }
}