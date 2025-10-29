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
 *  - to use console input use as: Read()
 *  - to use txt input use as: Read("file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as input folder
 *      ! also you can set here a random EXISTING! folder.
 *  
 *  Writing support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 *      ! basicly, it uses \\SciDevOOP\\bin\\Resources\\file_name as output folder
 *      ! also you can set here a random EXISTING! folder.
 */

partial class Program
{
    static void Main(string[] args)
    {
        IOptimizator optimizer = new MinimizerLevenbergMarquardt();
        var fun = new LineFunctionN();
        var points = Read("inputLineN.txt");
        var initial = new Vector
        {
            -1.0, 0.1, 2.0
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
        
        
        //TestLineN(optimizer, fun);
    }
}