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


    IVector IOptimizator.Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters, IVector maximumParameters)
    {
        var parameters = new Vector([.. initialParameters.Select(v => v)]);
        var minParameters = new Vector([.. initialParameters.Select(v => v)]);
        var f = function.Bind(parameters);
        var functionalMinimumValue = objective.Value(f);

        var rnd = new Random();
        uint i = 0; // Current iteration.
        var ti = double.MaxValue; // Current temperature.
        while (i < MaxIterations)
        {
            parameters.ForEach(elem => elem = rnd.NextDouble());
            ti = _temperatureChangeLaw.Value(ti, i);
            var transitionProbability = _transitionRule.Value(ti, objective, function, parameters, minimumParameters);
            if (transitionProbability != 0)
            {

                ++i;
            }
        }

        throw new NotImplementedException();
    }
}