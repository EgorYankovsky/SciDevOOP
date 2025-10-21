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
        var l2Value = ((IFunctional)this).Value(function);

        if (Math.Abs(l2Value) < 1e-15)
        {
            for (var i = 0; i < points.Count; i++)
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
        var Jacobian = new Matrix();
        if (function is IDifferentiableFunction differentiableFunction)
        {
            for (var i = 0; i < points.Count; i++)
            {
                var input = new Vector { points[i].x };
                var row = new Vector();
                var derivatives = differentiableFunction.Gradient(input);
                // ∂r_i/∂θ_j = ∂f(x_i)/∂θ_j
                for (var j = 0; j < derivatives.Count; j++)
                    row.Add(derivatives[j]);
                Jacobian.Add(row);
            }
        }
        return Jacobian;
    }

    IVector ILeastSquaresFunctional.Residual(IFunction function)
    {
        var residuals = new Vector();
        for (var i = 0; i < points.Count; i++)
        {
            var input = new Vector { points[i].x };
            var prediction = function.Value(input);
            residuals.Add(prediction - points[i].y);
        }
        return residuals;
    }

    double IFunctional.Value(IFunction function)
    {
        var sum = 0.0D;
        foreach (var (x, y) in points)
        {
            var param = new Vector() { x };
            var diff = function.Value(param) - y;
            sum += diff * diff;
        }
        return Math.Sqrt(sum);
    }
}