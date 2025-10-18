using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

interface IMatrixMultiplicand
{
    public IVector Multiplicate(IVector v);
    public IMatrix Multiplicate(IMatrix v);
}