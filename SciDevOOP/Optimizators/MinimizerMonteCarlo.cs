using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Optimizators;

class MinimizerMonteCarlo : IOptimizator
{
    public int MaxIterations = 100_000;

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        minimumParameters ??= new Vector([.. initialParameters.Select(v => 0)]);
        maximumParameters ??= new Vector([.. initialParameters.Select(v => 1)]);
        var param = new Vector([.. initialParameters.Select(p => p)]);
        var minParam = new Vector([.. initialParameters.Select(p => p)]);

        var fun = function.Bind(param);
        var currentMin = objective.Value(fun);
        
        var rand = new Random(0);

        for (var i = 0; i < MaxIterations; i++)
        {
            for (var j = 0; j < param.Count; j++) param[j] = minimumParameters[j] + (maximumParameters[j] - minimumParameters[j]) * rand.NextDouble();
            var f = objective.Value(function.Bind(param));
            if (f < currentMin)
            {
                currentMin = f;
                for (var j = 0; j < param.Count; j++) minParam[j] = param[j];
            }
        }
        return minParam;
    }
}