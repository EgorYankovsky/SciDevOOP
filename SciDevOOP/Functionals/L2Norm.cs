using SciDevOOP.Functions;
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
        // Для L2Norm Якобиан - это матрица частных производных невязок по параметрам
        // Если function параметрическая, нам нужно знать её производные

        var jacobian = new Matrix();

        // Проверяем, поддерживает ли функция вычисление производных
        if (function is IDifferentiableFunction differentiableFunction)
        {
            // function имеет методы для вычисления производных
            for (int i = 0; i < points.Count; i++)
            {
                var input = new Vector { points[i].x };
                var row = new Vector();

                // Вычисляем производные функции в точке x_i
                var derivatives = differentiableFunction.Gradient(input);

                // Для каждой невязки r_i = f(x_i) - y_i
                // ∂r_i/∂θ_j = ∂f(x_i)/∂θ_j
                for (int j = 0; j < derivatives.Count; j++)
                {
                    row.Add(derivatives[j]);
                }
                jacobian.Add(row);
            }
        }
        //else
        //{
        //    // Численное дифференцирование как запасной вариант
        //    jacobian = ComputeNumericalJacobianForL2Norm(function);
        //}

        return jacobian;
    }

    IVector ILeastSquaresFunctional.Residual(IFunction function)
    {
        var residuals = new Vector();
        for (var i = 0; i < points.Count; i++)
        {
            // Create input vector for function.
            var input = new Vector { points[i].x };
            // Account model's prediction.
            var prediction = function.Value(input);
            // Residual: prediction - actual
            residuals.Add(prediction - points[i].y);
        }
        return residuals;
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