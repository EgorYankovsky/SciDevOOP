using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Functionals;

class L2Norm : IDifferentiableFunctional, ILeastSquaresFunctional
{
    public IList<IList<double>>? points;

    IVector IDifferentiableFunctional.Gradient(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L2Norm");
        if (function is not IDifferentiableFunction)
            throw new ArgumentNullException("Can't find gradient of not IDifferentiableFunction.");
        var gradient = new Vector();
        var l2Value = ((IFunctional)this).Value(function);

        if (Math.Abs(l2Value) < 1e-15)
        {
            for (var i = 0; i < points.Count; i++)
                gradient.Add(0);
            return gradient;
        }

        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var diff = function.Value(param) - fi;
            gradient.Add(diff / l2Value);
        }
        return gradient;
    }

    IMatrix ILeastSquaresFunctional.Jacobian(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L2Norm");
        if (function is not IDifferentiableFunction)
            throw new ArgumentNullException("Can't find gradient of not IDifferentiableFunction.");
        var Jacobian = new Matrix();
        for (var i = 0; i < points.Count; i++)
        {
            var input = new Vector(points[i].SkipLast(1));
            var row = new Vector();
            var derivatives = (function as IDifferentiableFunction)!.Gradient(input);
            // ∂r_i/∂θ_j = ∂f(x_i)/∂θ_j
            for (var j = 0; j < derivatives.Count; j++)
                row.Add(derivatives[j]);
            Jacobian.Add(row);
        }
        return Jacobian;
    }

    IVector ILeastSquaresFunctional.Residual(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L2Norm");
        var residuals = new Vector();
        for (var i = 0; i < points.Count; i++)
        {
            var fi = points[i].Last();
            var input = new Vector(points[i].SkipLast(1));
            var prediction = function.Value(input);
            residuals.Add(prediction - fi);
        }
        return residuals;
    }

    double IFunctional.Value(IFunction function)
    {
        if (points is null) throw new ArgumentNullException("Points is null at L2Norm");
        var sum = 0.0D;
        foreach (var point in points)
        {
            var fi = point.Last();
            var param = new Vector(point.SkipLast(1));
            var diff = function.Value(param) - fi;
            sum += diff * diff;
        }
        return Math.Sqrt(sum);
    }
}