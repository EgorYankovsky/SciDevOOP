using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using System.Reflection;
using System.Text;

using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

var optimizer = new MinimizerMCGNew();
var fun = new LineFunctionN();
var initial = new Vector { 1.0, 1.0 };

var name = Assembly.GetExecutingAssembly().GetName().Name;
using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{name}.Resources.input.txt");
using var streamReader = new StreamReader(stream!, encoding: Encoding.UTF8);
var lines = streamReader.ReadToEnd().Split("\n");

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