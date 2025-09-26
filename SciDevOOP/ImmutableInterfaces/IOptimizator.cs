using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces;

/// <summary>
/// Optimizators interface.
/// </summary>
interface IOptimizator
{
    IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = default, IVector maximumParameters = default);
}