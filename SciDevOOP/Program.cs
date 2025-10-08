using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;


// 1. Выбор метода оптимизации.
var optimizer = new MinimizerMonteCarlo();

// 2. Начальные параметры для целевой функции.
var initial = new Vector { 1, 1};

// 3. Вводим точки, по которым строим сплайн (и прочее...).
var n = int.Parse(Console.ReadLine());
List<(double x, double y)> points = [];
for (var i = 0; i < n; i++)
{
    var str = Console.ReadLine()?.Split();
    points.Add((double.Parse(str[0]), double.Parse(str[1])));
}

// 4. Выбираем функционал. В качестве параметров обязательно нужны точки из п.3.
var functional = new L1Norm() { points = points };

// 5. Выбор целевой функции.
var fun = new LineFunction();

// 6. Решение задачи оптимизации.
var res = optimizer.Minimize(functional, fun, initial);

// 7. Вывод результата. (Целевые коэффициенты для функции).
foreach (var r in res)
    Console.WriteLine(r);