using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.GradientableAlgorithmTools;
using SciDevOOP.Optimizators.GradientableAlgorithmTools.LimitingMethods;
using SciDevOOP.Functions;

namespace SciDevOOP.Optimizators;

class MinimizerMCG : IOptimizator
{
    private IVector? _minimumParameters;
    private IVector? _maximumParameters;
    private IVector? _mesh;

    public int MaxIterations = 100_000;
    public double Tolerance = 1e-15;
    public double DichotomyEps = 1e-7;
    public double H = 1e-15; // For gradient.

    public ILimitingMethod? LimitingMethod;

    IVector IOptimizator.Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        IVector sln = new Vector();
        _mesh = (function.Bind(initialParameters) is IMeshable) ? (function.Bind(initialParameters) as IMeshable)!.GetMesh() : default;
        try
        {
            if (minimumParameters is not null && initialParameters.Count != minimumParameters.Count)
                throw new ArgumentException($"Minimum parameters amount {minimumParameters.Count} not equal to initial parameters amount {initialParameters.Count}.");
            if (maximumParameters is not null && initialParameters.Count != maximumParameters.Count)
                throw new ArgumentException($"Maximum parameters amount {maximumParameters.Count} not equal to initial parameters amount {initialParameters.Count}.");
            if (objective is not IDifferentiableFunctional)
                throw new ArgumentException($"MCG minimizer can't handle with {objective.GetType().Name} functional class.");

            (_minimumParameters, _maximumParameters) = (minimumParameters, maximumParameters);
            sln = Method(objective, function, initialParameters);
        }
        catch (ArgumentException argEx)
        {
            Console.WriteLine(argEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception:\n{ex}");
        }
        return new Vector(sln.Concat(_mesh is not null ? _mesh : []));
    }

    private IVector Method(IFunctional objective, IParametricFunction function, IVector initialParameters)
    {
        var k = 0;
        var xCurr = new Vector(initialParameters.Take(initialParameters.Count - (_mesh is not null ? _mesh.Count : 0)));
        var s = new Vector([.. xCurr.Select(p => 0)]);

        while (k < MaxIterations)
        {
            // step 1 - each n iterations zeros direction
            if (k % xCurr.Count == 0)
            {
                var grad = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(new Vector(xCurr.Concat(_mesh!))));
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
            var gradNext = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(new Vector(xNext.Concat(_mesh!))));
            var gradCurr = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(new Vector(xCurr.Concat(_mesh!))));

            var w = Math.Pow((gradNext as INormable)!.Norma(), 2) / Math.Pow((gradCurr as INormable)!.Norma(), 2);

            // Update direction s
            for (var i = 0; i < s.Count; i++)
                s[i] = -gradNext[i] + w * s[i];

            var sNorm = (s as INormable)!.Norma();
            var xDiff = new Vector();
            for (var i = 0; i < xCurr.Count; ++i) xDiff.Add(xNext[i] - xCurr[i]);
            var xDiffNorm = xDiff.Norma();
            if (k < 100) Console.WriteLine($"Current iteration: {k}. Current difference norm: {xDiffNorm:E15}. Current vector s norm: {sNorm:E15}");
            else if (k % 100 == 0) Console.WriteLine($"Current iteration: {k}. Current difference norm: {xDiffNorm:E15}. Current vector's norm: {sNorm:E15}");
            if (sNorm < Tolerance || xDiffNorm < Tolerance)
            {
                var a = 0; var b = 0;
                if (_minimumParameters is not null)
                    for (var i = 0; i < _minimumParameters.Count; ++i)
                        a += _minimumParameters[i] < xNext[i] ? 0 : 1;
                if (_maximumParameters is not null)
                    for (var i = 0; i < _maximumParameters.Count; ++i)
                        b += _maximumParameters[i] > xNext[i] ? 0 : 1;

                if (a + b == 0)
                {
                    Console.WriteLine($"MCG answer found for {k} iterations.");
                    return xNext;
                }
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

        for (var i = 0; i < 1000; ++i)
        {
            h *= 2;
            lambda += h;
            var f_prev = EvaluateFunction(objective, function, x, s, lambda - h);
            var f_curr = EvaluateFunction(objective, function, x, s, lambda);
            if (f_prev <= f_curr) break;
        }
        return (Math.Min(lambda0, lambda - h), Math.Max(lambda0, lambda));
    }

    private double DichotomyMethod(IFunctional objective, IParametricFunction function, (double, double) interval, IVector x, IVector s)
    {
        (var a, var b) = (interval.Item1, interval.Item2);
        (var x1, var x2) = (0.5 * (a + b - 0.5 * DichotomyEps), 0.5 * (a + b + 0.5 * DichotomyEps));
        while (Math.Abs(b - a) > DichotomyEps)
        {
            (var f_x1, var f_x2) = (EvaluateFunction(objective, function, x, s, x1), EvaluateFunction(objective, function, x, s, x2));
            if (f_x1 <= f_x2) b = x2;
            else a = x1;
            (x1, x2) = (0.5 * (a + b - 0.5 * DichotomyEps), 0.5 * (a + b + 0.5 * DichotomyEps));
        }
        return 0.5 * (x1 + x2);
    }

    private double EvaluateFunction(IFunctional objective, IParametricFunction function, IVector x, IVector s, double lambda)
    {
        var point = new Vector();
        for (var i = 0; i < x.Count; i++)
            point.Add(x[i] + lambda * s[i]);
        var fun = function.Bind(new Vector(point.Concat(_mesh!)));
        var value = objective.Value(fun);
        LimitingMethod ??= new BarrierMethod();
        LimitingMethod.Limit(ref value, point, _minimumParameters, _maximumParameters);
        return value;
    }
}