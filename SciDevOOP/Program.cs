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

var n = int.Parse(lines[0]);
List<(double x, double y)> points = [];
for (var i = 1; i <= n; i++)
{
    var str = lines[i].Split();
    points.Add((double.Parse(str[0]), double.Parse(str[1])));
}
var functional = new L1Norm { points = points };
var res = optimizer.Minimize(functional, fun, initial);
foreach (var r in res) Console.WriteLine(r);