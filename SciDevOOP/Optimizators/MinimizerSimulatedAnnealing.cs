using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools;

namespace SciDevOOP.Optimizators;

class MinimizerSimulatedAnnealing(ITransitionRule transitionRule, ITemperatureChangeLaw temperatureChangeLaw) : IOptimizator
{
    public readonly uint MaxIterations = 1000;
    public readonly double MinTemperature = 0.001;


    private readonly ITemperatureChangeLaw _temperatureChangeLaw = temperatureChangeLaw;
    private readonly ITransitionRule _transitionRule = transitionRule;


    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        minimumParameters ??= new Vector([.. initialParameters.Select(v => 0)]);
        maximumParameters ??= new Vector([.. initialParameters.Select(v => 1)]);
        var parameters = new Vector([.. initialParameters.Select(v => v)]);
        var minParameters = new Vector([.. initialParameters.Select(v => v)]);
        var f = function.Bind(parameters);
        var minFunctionalValue = objective.Value(f);

        var rnd = new Random();
        var ti = 100.0; // Current temperature.

        uint outerIter = 0;
        try
        {
            while (ti > MinTemperature)
            {
                uint i = 0; // Current iteration.
                while (i < MaxIterations)
                {
                    for (var ii = 0; ii < parameters.Count; ++ii)
                    {
                        var sign = Math.Pow(-1, rnd.Next(2));
                        parameters[ii] = minParameters[ii] + sign * (minimumParameters[ii] + (maximumParameters[ii] - minimumParameters[ii]) * rnd.NextDouble());
                    }

                    var newFunctionalValue = objective.Value(function.Bind(parameters));
                    var probability = _transitionRule.Value(ti, newFunctionalValue, minFunctionalValue);

                    if (rnd.NextDouble() < probability)
                    {
                        minFunctionalValue = newFunctionalValue;
                        minParameters = [.. parameters.Select(v => v)];
                    }
                    ++i;
                }
                outerIter++;
                ti = _temperatureChangeLaw.Value(ti, outerIter);
            }
            Console.WriteLine($"Total iterations: {outerIter * MaxIterations}");
        }
        catch (ArgumentException argEx)
        {
            Console.WriteLine($"Argument exception raised at MinimizerSimulatedAnnealing.\n{argEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error raised at MinimizerSimulatedAnnealing.\n{ex.Message}");
        }
        return minParameters; 
    }
}