using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.LevenbergMarquardtTools;

interface ISolver
{
    /// <summary>
    /// Method, that solves SLAE.
    /// </summary>
    /// <param name="A">Input matrix.</param>
    /// <param name="b">Input vector.</param>
    /// <returns>Vector of solution.</returns>
    public IVector Solve(IMatrix A, IVector b);
}