using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IFunction
{
    /// <summary>
    /// Method, that finds value of function at point. 
    /// </summary>
    /// <param name="point">Vector of point, where function will be calculated.</param>
    /// <returns>Function value.</returns>
    double Value(IVector point);
}