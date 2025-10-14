using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;


// 1. Выбор метода оптимизации.
var optimizer = new MinimizerLevenbergMarquardt();

// 2. Начальные параметры для целевой функции.
var initial = new Vector { 0, 0.5, 1.0, 1.0 };  

// 3. Вводим точки, по которым строим сплайн (и прочее...).
//var n = int.Parse(Console.ReadLine());
//List<(double x, double y)> points = [];
//for (var i = 0; i < n; i++)
//{
//    var str = Console.ReadLine()?.Split();
//    points.Add((double.Parse(str[0]), double.Parse(str[1])));
//}
var filePath = "input.txt";
var lines = File.ReadAllLines(filePath);

var n = int.Parse(lines[0]);
List<(double x, double y)> points = [];
for (var i = 1; i <= n; i++)
{
    var str = lines[i].Split();
    points.Add((double.Parse(str[0]), double.Parse(str[1])));
}

// 4. Выбираем функционал. В качестве параметров обязательно нужны точки из п.3.
var functional = new LInfNorm() { points = points };

// 5. Выбор целевой функции.
var fun = new PiecewiseLinearFunction();

// 6. Решение задачи оптимизации.
var res = optimizer.Minimize(functional, fun, initial);

// 7. Вывод результата. (Целевые коэффициенты для функции).
foreach (var r in res)
    Console.WriteLine(r);