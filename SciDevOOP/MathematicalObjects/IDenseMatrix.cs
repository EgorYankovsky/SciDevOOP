using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IDenseMatrix : IMatrix
{
    public int N { get; }
    public int M { get; }
    //public double this[int i, int j] { get; set; }
    public IMatrix GetTransposed();
}