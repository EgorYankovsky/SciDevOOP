using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class LInfNorm : IFunctional
{
    public IList<IList<double>>? points;

    double IFunctional.Value(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at LInfNorm");
        double max = 0;
        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var diff = Math.Abs(function.Value(param) - fi);
            if (diff > max) max = diff;
        }
        return max;
    }
}