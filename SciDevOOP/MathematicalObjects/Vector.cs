using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

public class Vector : List<double>, IVector
{
    public Vector() { }

    public Vector(IEnumerable<double> collection) : base(collection) {}
}