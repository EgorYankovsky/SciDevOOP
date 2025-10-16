using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using System;
using System.Collections.Generic;

namespace SciDevOOP.Optimizators;

class MinimizerLevenbergMarquardt : IOptimizator
{
    public int MaxIterations = 100;
    public double Tolerance = 1e-15;
    public double LambdaInit = 0.01;
    public double Nu = 10.0;
    public double H = 1e-8; // Step for numerical derivatives.

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
    {
        try
        {
            return objective is ILeastSquaresFunctional && objective is IDifferentiableFunctional
                ? LevenbergMarquardt(objective, function, initialParameters)
                : throw new ArgumentException();
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"MinimizerLevenbergMarquardt can't handle with {objective.GetType().Name} functional class.");
        }
        return new Vector();
    }

    private IVector LevenbergMarquardt(IFunctional objective, IParametricFunction function, IVector x0)
    {
        var n = x0.Count;
        var lambda = LambdaInit;
        var k = 0;

        // Account new vector residual and Jacobian.
        var residuals = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(x0));
        
        var J = ComputeJacobian(objective, function, x0, residuals.Count);

        var costCurr = ComputeCost(residuals);

        while (k < MaxIterations)
        {
            // Account gradient: J^T * r
            var gradient = ComputeGradient(J, residuals);
            //var gradient = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(x0));

            // Check gradient's norm.
            if (Norm(gradient) < Tolerance) return x0;
            
            // Hessian generation: J^T * J
            var JTJ = ComputeHessianApproximation(J, residuals.Count);

            // Add regularization parameters
            var A = AddLambdaToDiagonal(JTJ, lambda, n);

            // Solve system: (J^T * J + lambda * I) * h = -J^T * r
            var h = SolveLinearSystem(A, gradient, -1.0, n);

            // Supposed next step
            var nextX = new Vector();
            for (var i = 0; i < n; i++) nextX.Add(x0[i] + h[i]);

            // Account next residual 
            var residuals_next = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(nextX));

            // Account sum of residual squares.
            var cost_next = ComputeCost(residuals_next);

            // Account actual and supposed residual.
            var actualReduction = costCurr - cost_next;
            var predictedReduction = ComputePredictedReduction(J, gradient, h, lambda, residuals.Count);

            // Coefficient of step's quality.
            var rho = (predictedReduction != 0)
                ? actualReduction / predictedReduction
                : actualReduction;

            if (rho > 0.0001)
            {
                // Successful step - accept it.
                x0 = nextX;
                residuals = residuals_next;
                costCurr = cost_next;

                // Account Jacobian at new point
                J = ComputeJacobian(objective, function, x0, residuals.Count);

                // Reduce regularization parameter.
                lambda = Math.Max(lambda / Nu, 1e-16);
            }
            else
            {
                // Failed step - increase regularization parameter.
                lambda *= Nu;

                // Return if lambda too big.
                if (lambda > 1e16) return x0;
            }
            if (Norm(h) < Tolerance * (1 + Norm(x0))) return x0;
            if (Math.Abs(actualReduction) < Tolerance) return x0;
            k++;
        }
        return x0;
    }

    [Obsolete(message: "Method should be deleted.", false)]
    private IVector ComputeJacobian(IFunctional objective, IParametricFunction function, IVector parameters, int dataCount)
    {
        // Jacobian: residual's derivatives by parameters.
        var n = parameters.Count; // Parameters amount.
        var m = dataCount;        // Data points amount.

        var jacobian = new Vector();

        // Basic residuals.
        var baseResiduals = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(parameters));

        for (var j = 0; j < n; j++)
        {
            // Perturbations vector for j-th parameter.
            var perturbed = new Vector();
            for (var k = 0; k < n; k++)
                perturbed.Add(parameters[k]);
            perturbed[j] += H;

            // Residuals with perturbations.
            var perturbedResiduals = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(perturbed));

            // Account derivatives by j-th parameter.
            for (var i = 0; i < m; i++)
            {
                var derivative = (perturbedResiduals[i] - baseResiduals[i]) / H;
                jacobian.Add(derivative);
            }
        }
        return jacobian;
    }

    // It's just matrix and vector multiplication.
    [Obsolete(message: "Method should be deleted.", false)]
    private IVector ComputeGradient(IVector jacobian, IVector residuals)
    {
        // Gradient: J^T * r
        var m = residuals.Count; // Data points amount
        var n = jacobian.Count / m; // Parameters amount
        var gradient = new Vector();
        for (var j = 0; j < n; j++)
        {
            double sum = 0;
            for (var i = 0; i < m; i++)
                sum += jacobian[j * m + i] * residuals[i];
            gradient.Add(sum);
        }
        return gradient;
    }

    [Obsolete(message:"Change this method to matrices multiplication.", false)]
    private IVector ComputeHessianApproximation(IVector jacobian, int dataCount)
    {
        // Hessian approximation: J^T * J
        int m = dataCount; // число точек данных
        int n = jacobian.Count / m; // число параметров

        var hessian = new Vector();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < m; k++)
                {
                    sum += jacobian[i * m + k] * jacobian[j * m + k];
                }
                hessian.Add(sum);
            }
        }

        return hessian;
    }

    private IVector AddLambdaToDiagonal(IVector matrix, double lambda, int n)
    {
        var result = new Vector();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double value = matrix[i * n + j];
                if (i == j)
                    value += lambda;
                result.Add(value);
            }
        }

        return result;
    }

    private double ComputeCost(IVector residuals)
    {
        // Residual's squares
        double sum = 0;
        for (var i = 0; i < residuals.Count; i++)
            sum += residuals[i] * residuals[i];
        return 0.5 * sum; // 1/2 for derivative's simplification
    }

    private double ComputePredictedReduction(IVector J, IVector gradient, IVector h, double lambda, int dataCount)
    {
        // Предсказанное уменьшение: -h^T * J^T * r - 0.5 * h^T * (J^T * J + lambda * I) * h
        double linearTerm = 0;
        double quadraticTerm = 0;
        int n = h.Count;
        int m = dataCount;

        // -h^T * gradient (где gradient = J^T * r)
        for (int i = 0; i < n; i++)
        {
            linearTerm += -h[i] * gradient[i];
        }

        // 0.5 * h^T * (J^T * J + lambda * I) * h
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                // (J^T * J) часть
                double hessianElement = 0;
                for (int k = 0; k < m; k++)
                {
                    hessianElement += J[i * m + k] * J[j * m + k];
                }
                quadraticTerm += h[i] * hessianElement * h[j];
            }
            // lambda * I часть
            quadraticTerm += lambda * h[i] * h[i];
        }

        return linearTerm - 0.5 * quadraticTerm;
    }

    private IVector SolveLinearSystem(IVector A, IVector b, double scale, int n)
    {
        // Создаем расширенную матрицу
        var augmented = new double[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmented[i, j] = A[i * n + j];
            }
            augmented[i, n] = scale * b[i];
        }

        // Прямой ход метода Гаусса
        for (int i = 0; i < n; i++)
        {
            // Поиск главного элемента
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                    maxRow = k;
            }

            // Перестановка строк
            if (maxRow != i)
            {
                for (int k = 0; k <= n; k++)
                {
                    var temp = augmented[i, k];
                    augmented[i, k] = augmented[maxRow, k];
                    augmented[maxRow, k] = temp;
                }
            }

            // Проверка на вырожденность
            if (Math.Abs(augmented[i, i]) < 1e-15)
            {
                // Возвращаем нулевое решение
                var zeroSolution = new Vector();
                for (int idx = 0; idx < n; idx++)
                    zeroSolution.Add(0);
                return zeroSolution;
            }

            // Исключение
            for (int k = i + 1; k < n; k++)
            {
                double factor = augmented[k, i] / augmented[i, i];
                for (int j = i; j <= n; j++)
                {
                    augmented[k, j] -= factor * augmented[i, j];
                }
            }
        }

        // Обратный ход
        var solution = new Vector();
        for (int i = 0; i < n; i++)
            solution.Add(0);

        for (int i = n - 1; i >= 0; i--)
        {
            solution[i] = augmented[i, n];
            for (int j = i + 1; j < n; j++)
            {
                solution[i] -= augmented[i, j] * solution[j];
            }
            solution[i] /= augmented[i, i];
        }

        return solution;
    }

    private double Norm(IVector x)
    {
        double sum = 0;
        for (int i = 0; i < x.Count; i++)
            sum += x[i] * x[i];
        return Math.Sqrt(sum);
    }
}