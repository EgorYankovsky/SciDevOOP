using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Optimizators.LevenbergMarquardtTools;

interface ISolver
{
    public IVector Solve(IMatrix A, IVector b);
}