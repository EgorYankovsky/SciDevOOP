using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using System;

namespace SciDevOOP.Optimizators;

class MinimizerMCGNew : IOptimizator
{
    public int MaxIter = 100000;
    public double Tolerance = 1e-15;
    public double H = 1e-15; // For gradient.

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
    {
        IVector sln = new Vector();
        try
        {
            if (objective is not IDifferentiableFunctional)
                throw new ArgumentException();
            sln = Method(objective, function, initialParameters, 1e-15);
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"MCG minimizer can't handle with {objective.GetType().Name} functional class.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}");
        }
        return sln;
    }

    private IVector Method(IFunctional objective, IParametricFunction function, IVector initialParameters, double eps)
    {
        var k = 0;
        var xCurr = new Vector(initialParameters.Select(p => p));
        
        var s = new Vector();
        for (var i = 0; i < xCurr.Count; i++) s.Add(0);

        while (k < MaxIter)
        {
            // step 1 - each n iterations zeros direction
            if (k % xCurr.Count == 0)
            {
                var grad = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(xCurr));
                for (var i = 0; i < s.Count; i++)
                    s[i] = -grad[i];
            }

            // step 2 - find optimal step
            var lambda = FindMinLambda(objective, function, xCurr, s);

            // Update x
            var xNext = new Vector();
            for (var i = 0; i < xCurr.Count; i++)
                xNext.Add(xCurr[i] + lambda * s[i]);

            // step 3, 4 - find new direction.
            var gradNext = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(xNext));
            var gradCurr = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(xCurr));

            var w = Math.Pow((gradNext as INormable)!.Norma(), 2) / Math.Pow((gradCurr as INormable)!.Norma(), 2);

            // Update direction s
            for (var i = 0; i < s.Count; i++)
                s[i] = -gradNext[i] + w * s[i];

            var sNorm = (s as INormable)!.Norma();
            var xDiff = new Vector();
            for (var i = 0; i < xCurr.Count; ++i) xDiff.Add(xNext[i] - xCurr[i]);
            var xDiffNorm = xDiff.Norma();
            if (k < 100) Console.WriteLine($"Current iteration: {k}. Current difference norm: {xDiffNorm:E15}. Current vector s norm: {sNorm:E15}");
            if (k % 100 == 0) Console.WriteLine($"Current iteration: {k}. Current difference norm: {xDiffNorm:E15}. Current vector's norm: {sNorm:E15}");
            if (sNorm < eps || xDiffNorm < eps)
            {
                Console.WriteLine($"MCG answer found for {k} iterations.");
                return xNext;
            }
            xCurr = xNext;
            k++;
        }
        Console.WriteLine($"MCG reached max iterations: {k}");
        return xCurr;
    }

    private double FindMinLambda(IFunctional objective, IParametricFunction function, IVector x, IVector s)
    {
        var interval = FindMinimum(objective, function, x, s);
        return DichotomyMethod(objective, function, interval, x, s);
    }

    private (double, double) FindMinimum(IFunctional objective, IParametricFunction function, IVector x, IVector s, double lambda0 = 0.0)
    {
        var delta = 1e-8;
        var f_lambda0 = EvaluateFunction(objective, function, x, s, lambda0);
        var f_lambda_delta = EvaluateFunction(objective, function, x, s, lambda0 + delta);

        double lambda;
        double h;
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
        else return (lambda0, lambda0);

        while (true)
        {
            h *= 2;
            lambda += h;
            var f_prev = EvaluateFunction(objective, function, x, s, lambda - h);
            var f_curr = EvaluateFunction(objective, function, x, s, lambda);
            if (f_prev <= f_curr)
                return (Math.Min(lambda0, lambda - h), Math.Max(lambda0, lambda));
        }
    }

    private double DichotomyMethod(IFunctional objective, IParametricFunction function, (double, double) interval, IVector x, IVector s, double eps = 1e-7)
    {
        var a = interval.Item1;
        var b = interval.Item2;
        var x1 = 0.5 * (a + b - 0.5 * eps);
        var x2 = 0.5 * (a + b + 0.5 * eps);
        while (Math.Abs(b - a) > eps)
        {
            var f_x1 = EvaluateFunction(objective, function, x, s, x1);
            var f_x2 = EvaluateFunction(objective, function, x, s, x2);
            if (f_x1 <= f_x2) b = x2;
            else a = x1;
            x1 = (a + b - 0.5 * eps) / 2;
            x2 = (a + b + 0.5 * eps) / 2;
        }
        return 0.5 * (x1 + x2);
    }

    private double EvaluateFunction(IFunctional objective, IParametricFunction function, IVector x, IVector s, double lambda)
    {
        var point = new Vector();
        for (var i = 0; i < x.Count; i++)
            point.Add(x[i] + lambda * s[i]);
        var fun = function.Bind(point);
        return objective.Value(fun);
    }
}