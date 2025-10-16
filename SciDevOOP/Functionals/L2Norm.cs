using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class L2Norm : IDifferentiableFunctional, ILeastSquaresFunctional
{
    public List<(double x, double y)> points;

    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        var gradient = new Vector();
        double l2Value = ((IFunctional)this).Value(function);

        if (Math.Abs(l2Value) < 1e-15)
        {
            for (int i = 0; i < points.Count; i++)
                gradient.Add(0);
            return gradient;
        }

        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            gradient.Add(diff / l2Value);
        }
        return gradient;
    }

    IMatrix ILeastSquaresFunctional.Jacobian(IFunction function)
    {
        throw new NotImplementedException();
    }

    IVector ILeastSquaresFunctional.Residual(IFunction function)
    {
        var residual = new Vector();
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            residual.Add(diff);
        }
        return residual;
    }

    double IFunctional.Value(IFunction function)
    {
        double sum = 0;
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            sum += diff * diff;
        }
        return Math.Sqrt(sum);
    }
}