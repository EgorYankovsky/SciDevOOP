using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools;


namespace SciDevOOP.Optimizators;

class MinimizerSimulatedAnnealing(ITransitionRule transitionRule, ITemperatureChangeLaw temperatureChangeLaw) : IOptimizator
{
    public readonly uint MaxIterations = 100;
    public readonly double MinTemperature = 10.0;


    private readonly ITemperatureChangeLaw _temperatureChangeLaw = temperatureChangeLaw;
    private readonly ITransitionRule _transitionRule = transitionRule;


    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        var parameters = new Vector([.. initialParameters.Select(v => v)]);
        var minParameters = new Vector([.. initialParameters.Select(v => v)]);
        var f = function.Bind(parameters);
        var minFunctionalValue = objective.Value(f);

        var rnd = new Random();
        var ti = double.MaxValue; // Current temperature.

        uint outerIter = 0;
        while (ti > MinTemperature)
        {
            uint i = 0; // Current iteration.
            while (i < MaxIterations)
            {
                for (var ii = 0; ii < parameters.Count; ++ii)
                {
                    var sign = Math.Pow(-1, rnd.Next(2));
                    parameters[ii] += sign * rnd.NextDouble();
                }


                //parameters.ForEach(value => value += rnd.NextDouble());
                var newFunctionalValue = objective.Value(function.Bind(parameters));
                Console.WriteLine($"Functional value: {newFunctionalValue}");
                var probability = _transitionRule.Value(ti, newFunctionalValue, minFunctionalValue);

                if (rnd.NextDouble() < probability)
                {
                    minFunctionalValue = newFunctionalValue;
                    minParameters = parameters;
                }
            }
            ti = _temperatureChangeLaw.Value(ti, outerIter);
            outerIter++;
        }
        return minParameters; 
    }
}