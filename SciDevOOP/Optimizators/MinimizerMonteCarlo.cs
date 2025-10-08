using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Optimizators;

class MinimizerMonteCarlo : IOptimizator
{
    public int MaxIter = 100_000;
    
    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
    {
        // 1. Инициализация начальных данных для итерационного цикла.
        var param = new Vector();
        var minparam = new Vector();
        
        foreach (var p in initialParameters) param.Add(p);
        foreach (var p in initialParameters) minparam.Add(p);
        
        var fun = function.Bind(param);
        var currentmin = objective.Value(fun);
        
        var rand = new Random(0);

        // 2. Сам цикл решения.
        for (int i = 0; i < MaxIter; i++)
        {
            // Генерируем рандомные коэффициенты.
            for (int j = 0; j < param.Count; j++) param[j] = rand.NextDouble();
            // Находим значение функционала
            var f = objective.Value(function.Bind(param));
            // Обновляем параметры функции если значение функционала меньше.
            if (f < currentmin)
            {
                currentmin = f;
                for (int j = 0; j < param.Count; j++) minparam[j] = param[j];
            }
        }
        return minparam;
    }
}