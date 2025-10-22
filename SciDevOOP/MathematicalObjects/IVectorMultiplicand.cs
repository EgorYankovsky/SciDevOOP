using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IVectorMultiplicand
{
    /// <summary>
    /// Vector and vector multiplication method.
    /// </summary>
    /// <param name="v">Vector multiplier.</param>
    /// <returns>Result as scalar.</returns>
    public double Multiplicate(IVector v);
}