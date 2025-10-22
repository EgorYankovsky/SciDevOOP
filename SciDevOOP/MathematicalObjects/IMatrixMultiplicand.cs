using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IMatrixMultiplicand
{
    /// <summary>
    /// Matrix and vector multiplication method.
    /// </summary>
    /// <param name="v">Vector multiplier.</param>
    /// <returns>Result as vector</returns>
    public IVector Multiplicate(IVector v);

    /// <summary>
    /// Matrix and matrix multiplication method.
    /// </summary>
    /// <param name="v">Matrix multiplier.</param>
    /// <returns>Result as matrix</returns>
    public IMatrix Multiplicate(IMatrix v);
}