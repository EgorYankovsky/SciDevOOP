using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools;
using SciDevOOP.Functions;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;

namespace SciDevOOP.Optimizators;

class MinimizerSimulatedAnnealing : IOptimizator
{
    public uint MaxIterations = 100_000;
    public double MinTemperature = 0.0001;
    public double InitialTemperature = 1000.0D;
    public ITemperatureChangeLaw? TemperatureChangeLaw;
    public ITransitionRule? TransitionRule;


    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        TemperatureChangeLaw ??= new BasketFiring();
        TransitionRule ??= new ContinuousImprovement();

        minimumParameters ??= new Vector([.. initialParameters.Select(v => 0)]);
        maximumParameters ??= new Vector([.. initialParameters.Select(v => 1)]);
        var parameters = new Vector([.. initialParameters.Select(v => v)]);
        var minParameters = new Vector([.. initialParameters.Select(v => v)]);
        var f = function.Bind(parameters);
        var minFunctionalValue = objective.Value(f);

        var rnd = new Random();
        var ti = InitialTemperature; // Current temperature.

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
                    if (f is IMeshable)
                    {
                        var mesh = (f as IMeshable)!.GetMesh();
                        for (int ii = 1; ii <= mesh.Count; ++ii)
                            parameters[^ii] = mesh[^ii];
                    }

                    var newFunctionalValue = objective.Value(function.Bind(parameters));
                    var probability = TransitionRule?.Value(ti, newFunctionalValue, minFunctionalValue);

                    if (rnd.NextDouble() < probability)
                    {
                        minFunctionalValue = newFunctionalValue;
                        minParameters = [.. parameters.Select(v => v)];
                    }
                    ++i;
                }
                outerIter++;
                ti = TemperatureChangeLaw.Value(ti, outerIter);
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