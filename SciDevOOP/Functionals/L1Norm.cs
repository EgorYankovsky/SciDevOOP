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
        var gradient = new Vector();
        if (function is IDifferentiableFunction)
        {
            foreach (var (x, y) in points)
            {
                var param = new Vector { x };
                var diffSign = Math.Sign(y - function.Value(param));
                var gradVec = (function as IDifferentiableFunction)!.Gradient(param);
                gradient.Add(diffSign);
            }
        }
        return gradient;


        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            var derivative = diff > 0 ? 1 : (double)(diff < 0 ? -1 : 0);
            gradient.Add(derivative);
        }
        return gradient;
    }
}