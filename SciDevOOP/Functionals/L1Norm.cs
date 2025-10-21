using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class L1Norm : IDifferentiableFunctional
{
    public List<(double x, double y)> points;

    double IFunctional.Value(IFunction function)
    {
        double sum = 0;
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            sum += Math.Abs(diff);
        }
        return sum;
    }

    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        var gradient = new Vector();
        foreach (var (x, y) in points)
        {
            var param = new Vector { x };
            var gradF = (function as IDifferentiableFunction)!.Gradient(param);
            if (gradient.Count == 0)
                for (var i = 0; i < gradF.Count; ++i) gradient.Add(0);
            var sign = Math.Sign(y - function.Value(param));
            for (var i = 0; i < gradient.Count; ++i) gradient[i] += sign * gradF[i];
        }
        return gradient;
    }
}