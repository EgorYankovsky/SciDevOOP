using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.ImmutableInterfaces.Functions;

interface IParametricFunction
{
    /// <summary>
    /// Method, that binds parameters with generalized function.
    /// </summary>
    /// <param name="parameters">Binding parameters.</param>
    /// <returns>Function with bind parameters</returns>
    IFunction Bind(IVector parameters);
}