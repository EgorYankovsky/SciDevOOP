using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.LevenbergMarquardtTools;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;

namespace SciDevOOP.Optimizators;

class MinimizerLevenbergMarquardt : IOptimizator
{
    private IVector? _minimumParameters;
    private IVector? _maximumParameters;

    public int MaxIterations = 100;
    public double Tolerance = 1e-15;
    public double LambdaInit = 0.01;
    public double Nu = 10.0;
    public double H = 1e-8; // Step for numerical derivatives.
    public ISolver? Solver;

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        IVector sln = new Vector();
        try
        {
            if (minimumParameters is not null && minimumParameters.Count != initialParameters.Count)
                throw new ArgumentException($"Minimum parameters amount {minimumParameters.Count} not equal to initial parameters amount {initialParameters.Count}.");
            if (maximumParameters is not null && maximumParameters.Count != initialParameters.Count)
                throw new ArgumentException($"Maximum parameters amount {maximumParameters.Count} not equal to initial parameters amount {initialParameters.Count}.");
            if (objective is not ILeastSquaresFunctional || objective is not IDifferentiableFunctional)
                throw new ArgumentException($"Levenberg-Marquardt minimizer can't handle with {objective.GetType().Name} functional class.");
            (_minimumParameters, _maximumParameters) = (minimumParameters, maximumParameters);
            sln = LevenbergMarquardt(objective, function, initialParameters);
        }
        catch (ArgumentException argEx)
        {
            Console.WriteLine(argEx.Message);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Unexpected exception:\n{ex}");
        }
        return sln;
    }

    private IVector LevenbergMarquardt(IFunctional objective, IParametricFunction function, IVector x0)
    {
        var n = x0.Count;
        var lambda = LambdaInit;
        var k = 0;

        // Account new vector residual and Jacobian.
        var residuals = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(x0));
        var J = ((objective as ILeastSquaresFunctional)!.Jacobian(function.Bind(x0)) as IDenseMatrix)!.GetTransposed();
        var costCurr = 0.5 * (residuals as IVectorMultiplicand)!.Multiplicate(residuals);

        while (k < MaxIterations)
        {
            // Account gradient: J^T * r
            var gradient = (J as IMatrixMultiplicand)!.Multiplicate(residuals);

            // Check gradient's norm.
            if ((gradient as INormable)!.Norma() < Tolerance) return GetLimitedResult(x0);

            // Hessian generation: J^T * J
            var Jt = (J as IDenseMatrix)!.GetTransposed();
            var JTJ = (J as IMatrixMultiplicand)!.Multiplicate(Jt);


            // Add regularization parameters
            for (var i = 0; i < JTJ.Count; ++i) JTJ[i][i] += lambda;

            // Solve system: (J^T * J + lambda * I) * h = -J^T * r
            if (Solver is null)
            {
                Console.WriteLine("Cause Solver is null it set immediately as Gauss.");
                Solver = new NewGauss();
            }
            var h = Solver.Solve(JTJ, gradient);

            // Supposed next step
            var nextX = new Vector();
            for (var i = 0; i < n; i++) nextX.Add(x0[i] + h[i]);

            // Account next residual 
            var nextResidual = (objective as ILeastSquaresFunctional)!.Residual(function.Bind(nextX));

            // Account sum of residual squares.
            var nextCost = 0.5 * (nextResidual as IVectorMultiplicand)!.Multiplicate(nextResidual); // ComputeCost(residuals_next);

            // Account actual and supposed residual.
            var actualReduction = costCurr - nextCost;
            var predictedReduction = ComputePredictedReduction(J, gradient, h, lambda, residuals.Count);

            // Coefficient of step's quality.
            var rho = (predictedReduction != 0)
                ? actualReduction / predictedReduction
                : actualReduction;

            if (rho > 0.0001)
            {
                // Successful step - accept it.
                x0 = nextX;
                residuals = nextResidual;
                costCurr = nextCost;
                // Account Jacobian at new point
                J = ((objective as ILeastSquaresFunctional)!.Jacobian(function.Bind(x0)) as IDenseMatrix)!.GetTransposed();
                // Reduce regularization parameter.
                lambda = Math.Max(lambda / Nu, 1e-16);
            }
            else
            {
                // Failed step - increase regularization parameter.
                lambda *= Nu;
                // Return if lambda too big.
                if (lambda > 1e16) return GetLimitedResult(x0);
            }
            if ((h as INormable)!.Norma() < Tolerance * (1 + (x0 as INormable)!.Norma())) return GetLimitedResult(x0);
            if (Math.Abs(actualReduction) < Tolerance) return GetLimitedResult(x0);
            k++;
        }
        return GetLimitedResult(x0);
    }


    /// <summary>
    /// Method, that limits result vector.
    /// </summary>
    /// <param name="ans">Not limited result vector.</param>
    /// <returns>Vector with minimal and maximal limitations.</returns>
    private IVector GetLimitedResult(IVector ans)
    {
        if (_minimumParameters is not null)
            for (var i = 0; i < ans.Count; ++i)
                if (ans[i] < _minimumParameters[i])
                    ans[i] = _minimumParameters[i];
        if (_maximumParameters is not null)
            for (var i = 0; i < ans.Count; ++i)
                if (ans[i] > _maximumParameters[i])
                    ans[i] = _maximumParameters[i];
        return ans;
    }

    private double ComputePredictedReduction(IMatrix J, IVector gradient, IVector h, double lambda, int dataCount)
    {
        // Predicted reduction: -h^T * J^T * r - 0.5 * h^T * (J^T * J + lambda * I) * h
        var linearTerm = 0.0D;
        var quadraticTerm = 0.0D;
        var n = h.Count;
        var m = dataCount;

        // -h^T * gradient (where gradient = J^T * r)
        for (var i = 0; i < n; i++)
            linearTerm += -h[i] * gradient[i];

        // 0.5 * h^T * (J^T * J + lambda * I) * h
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                // (J^T * J)
                double hessianElement = 0;
                for (var k = 0; k < m; k++)
                    hessianElement += J[i][k] * J[j][k];
                quadraticTerm += h[i] * hessianElement * h[j];
            }
            // lambda * I
            quadraticTerm += lambda * h[i] * h[i];
        }
        return linearTerm - 0.5 * quadraticTerm;
    }
}