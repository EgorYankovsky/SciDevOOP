using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;

var linefN = new LineFunctionN();
var pwlf = new PiecewiseLinearFunction();
var plf = new Polynomial();

//var testLinefN = linefN.Bind(new Vector() { 4.0, 3.0, 7.0, -8.0, 1.0, -0.5 });
var testpwlf = pwlf.Bind(new Vector() { 1.1, 2.2, 3.3, 4.4, 5.5, 
                                        1.0, 1.2, 1.3, 1.4, 1.5, 1.6, 
                                       -1.0, -2.0, -3.0, -4.0, -5.0, -6.0 });

double h = 0.05;
double x0 = 0.0;
double xi = x0;
double xn = 6.0;

int ii = 0;
while (xi < xn)
{
    Console.WriteLine($"{testpwlf.Value(new Vector() { xi })}");
    xi = x0 + h * (++ii);
}

return 0;
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