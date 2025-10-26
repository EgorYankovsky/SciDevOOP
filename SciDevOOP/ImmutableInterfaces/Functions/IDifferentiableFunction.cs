using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IDifferentiableFunction : IFunction
{
    /// <summary>
    /// Method, that finds gradient of function's parameters.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Vector of partial derivatives.</returns>
    IVector Gradient(IVector point);
}