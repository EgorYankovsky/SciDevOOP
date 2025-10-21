using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using System.Reflection;
using System.Text;

using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

/*
var A = new Matrix(4, 4);
var b = new Vector { 19, 85, 112, 161 };

A[0][0] = 1; A[0][1] = 2; A[0][2] = 3; A[0][3] = 4;
A[1][0] = 2; A[1][1] = 8; A[1][2] = 13; A[1][3] = 20;
A[2][0] = 3; A[2][1] = 13; A[2][2] = 18; A[2][3] = 25;
A[3][0] = 4; A[3][1] = 20; A[3][2] = 25; A[3][3] = 36;

var slvr = new NewGauss();

var x = slvr.Solve(A, b);
foreach (var xi in x)
    Console.WriteLine(xi);
return 0;
*/

var optimizer = new MinimizerLevenbergMarquardt();
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
var functional = new L2Norm { points = points };
var res = optimizer.Minimize(functional, fun, initial);
foreach (var r in res) Console.WriteLine(r);