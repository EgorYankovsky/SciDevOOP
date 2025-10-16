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
    public int MaxIter = 100;
    public double Tolerance = 1e-15;
    public double LambdaInit = 0.01;
    public double Nu = 10.0;
    public double H = 1e-8; // для вычисления производных

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
    {
        var x = new Vector();
        foreach (var p in initialParameters) x.Add(p);

        return LevenbergMarquardt(objective, function, x, Tolerance);
    }

    private IVector LevenbergMarquardt(IFunctional objective, IParametricFunction function, IVector x0, double eps)
    {
        int n = x0.Count;
        var x = new Vector();
        foreach (var p in x0) x.Add(p);

        double lambda = LambdaInit;
        int k = 0;

        // Вычисляем начальные невязки и якобиан
        Vector residuals = ComputeResiduals(objective, function, x);
        var J = ComputeJacobian(objective, function, x, residuals.Count);

        double cost_curr = ComputeCost(residuals);

        while (k < MaxIter)
        {
            // Вычисляем градиент: J^T * r
            var gradient = ComputeGradient(J, residuals);

            // Проверка сходимости по норме градиента
            if (Norm(gradient) < eps)
            {
                return x;
            }

            // Создаем матрицу Гессе приближение: J^T * J
            //var JTJ = ComputeHessianApproximation(J, residuals.Length);
            var JTJ = J.GetTransposed() * J;

            // Добавляем параметр регуляризации к диагонали
            AddLambdaToDiagonal(JTJ, lambda, n);

            // Решаем систему: (J^T * J + lambda * I) * h = -J^T * r
            var h = SolveLinearSystem(JTJ, gradient, -1.0, n);

            // Предлагаемый следующий шаг
            var x_next = new Vector();
            for (int i = 0; i < n; i++)
                x_next.Add(x[i] + h[i]);

            // Вычисляем невязки в новой точке
            var residuals_next = ComputeResiduals(objective, function, x_next);

            // Вычисляем сумму квадратов невязок
            double cost_next = ComputeCost(residuals_next);

            // Вычисляем фактическое и предсказанное уменьшение
            double actualReduction = cost_curr - cost_next;
            double predictedReduction = ComputePredictedReduction(J, gradient, h, lambda, residuals.Count);

            // Коэффициент качества шага
            double rho = (predictedReduction != 0)
                ? actualReduction / predictedReduction
                : actualReduction;

            if (rho > 0.0001)
            {
                // Шаг успешен - принимаем его
                x = x_next;
                residuals = residuals_next;
                cost_curr = cost_next;

                // Пересчитываем якобиан в новой точке
                J = ComputeJacobian(objective, function, x, residuals.Count);

                // Уменьшаем параметр регуляризации
                lambda = Math.Max(lambda / Nu, 1e-16);
            }
            else
            {
                // Шаг неудачен - увеличиваем параметр регуляризации
                lambda *= Nu;

                // Если lambda становится слишком большой, выходим
                if (lambda > 1e16)
                {
                    return x;
                }
            }

            // Дополнительный критерий остановки по изменению параметров
            if (Norm(h) < eps * (1 + Norm(x)))
            {
                return x;
            }

            // Критерий остановки по уменьшению стоимости
            if (Math.Abs(actualReduction) < eps)
            {
                return x;
            }

            k++;
        }

        return x;
    }

    private Vector ComputeResiduals(IFunctional objective, IParametricFunction function, IVector parameters)
    {
        // Получаем функцию с текущими параметрами
        var fun = function.Bind(parameters);

        // Используем функционал для вычисления невязок
        // Предполагаем, что функционал L2Norm имеет доступ к точкам данных
        var pointsField = objective.GetType().GetField("points");
        if (pointsField != null)
        {
            var points = pointsField.GetValue(objective) as List<(double x, double y)>;
            if (points != null)
            {
                var residuals = new Vector(points.Count);

                for (int i = 0; i < points.Count; i++)
                {
                    // Создаем входной вектор для функции
                    var input = new Vector();
                    input.Add(points[i].x);

                    // Вычисляем предсказание модели
                    double prediction = fun.Value(input);
                    // Невязка: prediction - actual
                    residuals.Add(prediction - points[i].y);
                }

                return residuals;
            }
        }

        throw new InvalidOperationException("Cannot access data points from functional");
    }

    private Matrix ComputeJacobian(IFunctional objective, IParametricFunction function, IVector parameters, int dataCount)
    {
        // Якобиан: производные невязок по параметрам
        int n = parameters.Count; // число параметров
        int m = dataCount;        // число точек данных

        var jacobian = new Matrix(n, m);

        // Базовые невязки
        var baseResiduals = ComputeResiduals(objective, function, parameters);

        for (int j = 0; j < n; j++)
        {
            // Вектор с возмущением по j-му параметру
            var perturbed = new Vector();
            for (int k = 0; k < n; k++)
                perturbed.Add(parameters[k]);
            perturbed[j] += H;

            // Невязки с возмущением
            var perturbedResiduals = ComputeResiduals(objective, function, perturbed);

            // Вычисляем производные невязок по j-му параметру
            for (int i = 0; i < m; i++)
            {
                double derivative = (perturbedResiduals[i] - baseResiduals[i]) / H;
                jacobian[j, i] = derivative;
            }
        }

        return jacobian;
    }

    private IVector ComputeGradient(Matrix jacobian, Vector residuals)
    {
        var gradient = jacobian * residuals;        
        return gradient;
    }

    //private IVector ComputeHessianApproximation(IMatrix jacobian, int dataCount)
    //{
    //    // Hessian approximation: J^T * J
    //    int m = dataCount; // число точек данных
    //    int n = jacobian.Count / m; // число параметров

    //    var hessian = new Vector();

    //    for (int i = 0; i < n; i++)
    //    {
    //        for (int j = 0; j < n; j++)
    //        {
    //            double sum = 0;
    //            for (int k = 0; k < m; k++)
    //            {
    //                sum += jacobian[i * m + k] * jacobian[j * m + k];
    //            }
    //            hessian.Add(sum);
    //        }
    //    }

    //    return hessian;
    //}

    private void AddLambdaToDiagonal(Matrix matrix, double lambda, int n)
    {
        for (int i = 0; i < n; i++)
            matrix[i,i] += lambda;
    }

    private double ComputeCost(Vector residuals)
    {
        // Сумма квадратов невязок
        double sum = 0;
        for (int i = 0; i < residuals.Count; i++)
        {
            sum += residuals[i] * residuals[i];
        }
        return 0.5 * sum; // 1/2 для упрощения производных
    }

    private double ComputePredictedReduction(Matrix J, IVector gradient, IVector h, double lambda, int dataCount)
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
                    hessianElement += J[i, k] * J[j, k];
                }
                quadraticTerm += h[i] * hessianElement * h[j];
            }
            // lambda * I часть
            quadraticTerm += lambda * h[i] * h[i];
        }

        return linearTerm - 0.5 * quadraticTerm;
    }

    private IVector SolveLinearSystem(Matrix A, IVector b, double scale, int n)
    {
        // Создаем расширенную матрицу
        var augmented = new double[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmented[i, j] = A[i, j];
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