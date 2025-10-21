using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IDenseMatrix : IMatrix
{
    public int N { get; }
    public int M { get; }
    public IMatrix GetTransposed();
}