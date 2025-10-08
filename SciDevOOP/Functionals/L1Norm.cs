using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

public class L1Norm : IDifferentiableFunctional
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
            var param = new Vector() { x };
            var diff = function.Value(param) - y;

            double derivative;
            if (diff > 0)
                derivative = 1;
            else if (diff < 0)
                derivative = -1;
            else
                derivative = 0;
            gradient.Add(derivative);
        }
        return gradient;
        //throw new NotImplementedException();
    }
}