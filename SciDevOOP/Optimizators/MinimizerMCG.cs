using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using System;

namespace SciDevOOP.Optimizators;

class MinimizerMCG : IOptimizator
{
    public int MaxIter = 100000;
    public double Tolerance = 1e-15;
    public double H = 1e-15; // для вычисления градиента

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
    {
        var x0 = new Vector();
        foreach (var p in initialParameters) x0.Add(p);

        return Method(objective, function, x0, Tolerance);
    }

    private IVector Method(IFunctional objective, IParametricFunction function, IVector x0, double eps)
    {
        int k = 0;
        var x_curr = new Vector();
        foreach (var p in x0) x_curr.Add(p);

        var s = new Vector();
        for (int i = 0; i < x_curr.Count; i++) s.Add(0);

        while (k < MaxIter)
        {
            // step 1 - каждые n итераций сбрасываем направление
            if (k % x_curr.Count == 0)
            {
                var grad = ComputeGradient(objective, function, x_curr);
                for (int i = 0; i < s.Count; i++)
                    s[i] = -grad[i];
            }

            // step 2 - поиск оптимального шага
            double lambda = FindMinLambda(objective, function, x_curr, s);

            // Обновляем x
            var x_next = new Vector();
            for (int i = 0; i < x_curr.Count; i++)
                x_next.Add(x_curr[i] + lambda * s[i]);

            // step 3, 4 - вычисление нового направления
            var grad_next = ComputeGradient(objective, function, x_next);
            var grad_curr = ComputeGradient(objective, function, x_curr);

            double w = Math.Pow(Norm(grad_next), 2) / Math.Pow(Norm(grad_curr), 2);

            // Обновляем направление s
            for (int i = 0; i < s.Count; i++)
                s[i] = -grad_next[i] + w * s[i];

            // Проверка критерия остановки
            if (Norm(s) < eps)
            {
                Console.WriteLine($"{k} MCG answer found");
                return x_next;
            }

            x_curr = x_next;
            k++;
        }

        Console.WriteLine($"MCG reached max iterations: {k}");
        return x_curr;
    }

    private double Norm(IVector x)
    {
        double sum = 0;
        for (int i = 0; i < x.Count; i++)
            sum += x[i] * x[i];
        return Math.Sqrt(sum);
    }

    private IVector ComputeGradient(IFunctional objective, IParametricFunction function, IVector x)
    {
        var gradient = new Vector();

        // Базовое значение функции
        var baseFun = function.Bind(x);
        double baseValue = objective.Value(baseFun);

        for (int i = 0; i < x.Count; i++)
        {
            // perturbed vector
            var perturbed = new Vector();
            foreach (var p in x) perturbed.Add(p);
            perturbed[i] += H;

            var perturbedFun = function.Bind(perturbed);
            double perturbedValue = objective.Value(perturbedFun);

            gradient.Add((perturbedValue - baseValue) / H);
        }

        return gradient;
    }

    private double FindMinLambda(IFunctional objective, IParametricFunction function, IVector x, IVector s)
    {
        var interval = FindMinimum(objective, function, x, s);
        return DichotomyMethod(objective, function, interval, x, s);
    }

    private (double, double) FindMinimum(IFunctional objective, IParametricFunction function, IVector x, IVector s, double lambda0 = 0.0)
    {
        double h = 0;
        double lambda = lambda0;
        double delta = 1e-8;

        double f_lambda0 = EvaluateFunction(objective, function, x, s, lambda0);
        double f_lambda_delta = EvaluateFunction(objective, function, x, s, lambda0 + delta);

        if (f_lambda0 > f_lambda_delta)
        {
            lambda = lambda0 + delta;
            h = delta;
        }
        else if (f_lambda0 < f_lambda_delta)
        {
            lambda = lambda0 + delta;
            h = -delta;
        }
        else
        {
            return (lambda0, lambda0);
        }

        while (true)
        {
            h *= 2;
            lambda += h;

            double f_prev = EvaluateFunction(objective, function, x, s, lambda - h);
            double f_curr = EvaluateFunction(objective, function, x, s, lambda);

            if (f_prev <= f_curr)
            {
                return (Math.Min(lambda0, lambda - h), Math.Max(lambda0, lambda));
            }
        }
    }

    private double DichotomyMethod(IFunctional objective, IParametricFunction function, (double, double) interval, IVector x, IVector s, double eps = 1e-7)
    {
        double a = interval.Item1;
        double b = interval.Item2;
        double x1 = (a + b - 0.5 * eps) / 2;
        double x2 = (a + b + 0.5 * eps) / 2;

        while (Math.Abs(b - a) > eps)
        {
            double f_x1 = EvaluateFunction(objective, function, x, s, x1);
            double f_x2 = EvaluateFunction(objective, function, x, s, x2);

            if (f_x1 <= f_x2)
            {
                b = x2;
            }
            else
            {
                a = x1;
            }

            x1 = (a + b - 0.5 * eps) / 2;
            x2 = (a + b + 0.5 * eps) / 2;
        }

        return 0.5 * (x1 + x2);
    }

    private double EvaluateFunction(IFunctional objective, IParametricFunction function, IVector x, IVector s, double lambda)
    {
        var point = new Vector();
        for (int i = 0; i < x.Count; i++)
            point.Add(x[i] + lambda * s[i]);

        var fun = function.Bind(point);
        return objective.Value(fun);
    }
}