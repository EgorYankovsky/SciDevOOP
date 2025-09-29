using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;

var optimizer = new MinimizerMonteCarlo();
var initial = new Vector { 0.5,
                           0.1, 0.2,
                           0.1, 0.1,
                           0.006, 0.004};
var n = int.Parse(Console.ReadLine());
List<(double x, double y)> points = [];
for (var i = 0; i < n; i++)
{
    var str = Console.ReadLine()?.Split();
    points.Add((double.Parse(str[0]), double.Parse(str[1])));
}
var functional = new MyFunctional() { points = points };
var fun = new PiecewiseLinearFunction();

var res = optimizer.Minimize(functional, fun, initial);

foreach (var r in res)
    Console.WriteLine(r);