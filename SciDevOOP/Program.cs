using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;

var optimizer = new MinimizerMonteCarlo();
var initial = new Vector { 1, 1 };
var n = int.Parse(Console.ReadLine());
List<(double x, double y)> points = [];
for (var i = 0; i < n; i++)
{
    var str = Console.ReadLine()?.Split();
    points.Add((double.Parse(str[0]), double.Parse(str[1])));
}
var functional = new MyFunctional() { points = points };
var fun = new LineFunction();

var res = optimizer.Minimize(functional, fun, initial);
Console.WriteLine($"a={res[0]},b={res[1]}");