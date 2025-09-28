using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class MyFunctional : IFunctional
{
    public List<(double x, double y)> points;
    public double Value(IFunction function)
    {
        double sum = 0;
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var s = function.Value(param) - y;
            sum += s * s;
        }
        return sum;
    }
}