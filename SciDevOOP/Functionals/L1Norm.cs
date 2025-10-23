using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class L1Norm : IDifferentiableFunctional
{
    public IList<IList<double>>? points;

    double IFunctional.Value(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L1Norm");
        double sum = 0;
        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var diff = function.Value(param) - fi;
            sum += Math.Abs(diff);
        }
        return sum;
    }

    //IVector IDifferentiableFunctional.Gradient(IFunction function)
    //{
    //    var gradient = new Vector();
    //
    //
    //
    //
    //    foreach (var (x, y) in points)
    //    {
    //        var param = new Vector() { x };
    //        var diff = function.Value(param) - y;
    //        var derivative = diff > 0 ? 1 : (double)(diff < 0 ? -1 : 0);
    //        gradient.Add(derivative);
    //    }
    //    return gradient;
    //}

    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L1Norm");
        var gradient = new Vector();
        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var gradF = (function as IDifferentiableFunction)!.Gradient(param);
            if (gradient.Count == 0)
                for (var i = 0; i < gradF.Count; ++i) gradient.Add(0);
            var sign = Math.Sign(fi - function.Value(param));
            for (var i = 0; i < gradient.Count; ++i) gradient[i] += sign * gradF[i];
        }
        return gradient;
    }
}