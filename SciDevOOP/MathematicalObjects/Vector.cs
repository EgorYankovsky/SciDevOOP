using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.MathematicalObjects;

public class Vector : List<double>, INormable
{
    public Vector() : base() { }

    public Vector(int  capacity) : base(capacity) { }

    public Vector(IEnumerable<double> collection) : base(collection) {}

    public double Norma()
    {
        var sum = 0.0;
        for (var i = 0; i < Count; ++i)
            sum += this[i] * this[i];
        return Math.Sqrt(sum);
    }
}