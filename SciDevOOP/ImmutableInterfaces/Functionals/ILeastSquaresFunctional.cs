using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functionals;

interface ILeastSquaresFunctional : IFunctional
{
    /// <summary>
    /// Method, that calculates vector of residual.
    /// </summary>
    /// <param name="function">Calculated function.</param>
    /// <returns>Vector of residuals.</returns>
    IVector Residual(IFunction function);
    
    /// <summary>
    /// Method, that finds Jacobian of selected function.
    /// </summary>
    /// <param name="function">Calculated function.</param>
    /// <returns>Jacobi matrix of derivatives.</returns>
    IMatrix Jacobian(IFunction function);
}