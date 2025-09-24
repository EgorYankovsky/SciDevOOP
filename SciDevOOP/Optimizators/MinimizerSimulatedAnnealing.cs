using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.Interfaces;

namespace SciDevOOP.Optimizators;

public class MinimizerSimulatedAnnealing : IOptimizator
{
    IVector IOptimizator.Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters, IVector maximumParameters)
    {
        throw new NotImplementedException();
    }
}