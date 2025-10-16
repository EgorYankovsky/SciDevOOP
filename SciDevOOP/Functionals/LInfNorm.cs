using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class LInfNorm : IFunctional
{
    public List<(double x, double y)> points;

    double IFunctional.Value(IFunction function)
    {
        double max = 0;
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = Math.Abs(function.Value(param) - y);
            if (diff > max)
                max = diff;
        }
        return max;
    }
}