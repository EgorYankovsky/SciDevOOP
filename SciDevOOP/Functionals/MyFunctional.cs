using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class MyFunctional : IFunctional
{
    public IList<IList<double>>? points;

    public double Value(IFunction function)
    {
        if (points == null) throw new ArgumentNullException("Points is null at MyFunctional");
        double sum = 0;
        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var s = function.Value(param) - fi;
            sum += s * s;
        }
        return sum;
    }
}