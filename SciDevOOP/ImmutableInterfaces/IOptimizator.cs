using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces;

/// <summary>
/// Optimizators interface.
/// </summary>
interface IOptimizator
{
    /// <summary>
    /// Method, that solves minimization problem.
    /// </summary>
    /// <param name="objective">Computational functional</param>
    /// <param name="function">Function, that should be minimized</param>
    /// <param name="initialParameters">Initial parameters for function</param>
    /// <param name="minimumParameters">Minimal parameters for function</param>
    /// <param name="maximumParameters">Maximal parameters for function</param>
    /// <returns>Vector of minimized parameters for function</returns>
    IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = default, IVector maximumParameters = default);
}