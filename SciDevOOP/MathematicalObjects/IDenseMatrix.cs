using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IDenseMatrix : IMatrix
{
    /// <summary>
    /// Matrix's rows amount.
    /// </summary>
    public int N { get; }

    /// <summary>
    /// Matrix's columns amount
    /// </summary>
    public int M { get; }
    
    /// <summary>
    /// Method, that transpose matrix.
    /// </summary>
    /// <returns>Transposed matrix.</returns>
    public IMatrix GetTransposed();
}