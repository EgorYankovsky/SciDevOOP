using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.LevenbergMarquardtTools;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;
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
    public ISolver? Solver;


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

        var costCurr = 0.5 * (residuals as IVectorMultiplicand)!.Multiplicate(residuals); // ComputeCost(residuals);

        while (k < MaxIterations)
        {
            // Account gradient: J^T * r
            var gradient = (J as IMatrixMultiplicand)!.Multiplicate(residuals);
            //var gradient = (objective as IDifferentiableFunctional)!.Gradient(function.Bind(x0));

            // Check gradient's norm.
            if ((gradient as INormable)!.Norma() < Tolerance) return x0;

            // Hessian generation: J^T * J
            //var JTJ = ComputeHessianApproximation(J, residuals.Count);
            var Jt = (J as IDenseMatrix)!.GetTransposed();
            var JTJ = (J as IMatrixMultiplicand)!.Multiplicate(Jt);


            // Add regularization parameters
            //var A = AddLambdaToDiagonal(JTJ, lambda, n);
            for (var i = 0; i < JTJ.Count; ++i) JTJ[i][i] += lambda;


            // Solve system: (J^T * J + lambda * I) * h = -J^T * r
            //var h = SolveLinearSystem(JTJ, gradient, -1.0, n);
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
            //if (Norm(h) < Tolerance * (1 + Norm(x0))) return x0;
            if ((h as INormable)!.Norma() < Tolerance * (1 + (x0 as INormable)!.Norma())) return x0;
            if (Math.Abs(actualReduction) < Tolerance) return x0;
            k++;
        }
        return x0;
    }

    private IMatrix ComputeJacobian(IFunctional objective, IParametricFunction function, IVector parameters, int dataCount)
    {
        // Jacobian: residual's derivatives by parameters.
        var n = parameters.Count; // Parameters amount.
        var m = dataCount;        // Data points amount.

        var jacobian = new Matrix(n, m);

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
                jacobian[j][i] = derivative;
            }
        }
        return jacobian;
    }

#if BEER
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
#endif

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

    [Obsolete(message:"Change this method to matrices multiplication.", true)]
    private IVector ComputeHessianApproximation(IVector jacobian, int dataCount)
    {
        // Hessian approximation: J^T * J
        var m = dataCount; // Data points amount
        var n = jacobian.Count / m; // Parameters amount
        var hessian = new Vector();
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                var sum = 0.0D;
                for (var k = 0; k < m; k++)
                    sum += jacobian[i * m + k] * jacobian[j * m + k];
                hessian.Add(sum);
            }
        }
        return hessian;
    }

    [Obsolete("Method must be replaced with matrix's manipulations", false)]
    private IVector AddLambdaToDiagonal(IVector matrix, double lambda, int n)
    {
        var result = new Vector();
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                var value = matrix[i * n + j];
                if (i == j)
                    value += lambda;
                result.Add(value);
            }
        }
        return result;
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


#if BEER
    private double ComputePredictedReduction(IVector J, IVector gradient, IVector h, double lambda, int dataCount)
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
                    hessianElement += J[i * m + k] * J[j * m + k];
                quadraticTerm += h[i] * hessianElement * h[j];
            }
            // lambda * I
            quadraticTerm += lambda * h[i] * h[i];
        }
        return linearTerm - 0.5 * quadraticTerm;
    }
#endif

    private IVector SolveLinearSystem(IVector A, IVector b, double scale, int n)
    {
        // Create augmented matrix
        var augmented = new double[n, n + 1];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
                augmented[i, j] = A[i * n + j];
            augmented[i, n] = scale * b[i];
        }

        // Gauss forward elimination
        for (var i = 0; i < n; i++)
        {
            // Search for the main element
            var maxRow = i;
            for (var k = i + 1; k < n; k++)
            {
                if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                    maxRow = k;
            }

            // Swap rows
            if (maxRow != i)
                for (var k = 0; k <= n; k++)
                    (augmented[maxRow, k], augmented[i, k]) = (augmented[i, k], augmented[maxRow, k]);


            // Degeneracy check
            if (Math.Abs(augmented[i, i]) < 1e-15)
            {
                // Return zeros
                var zeroSolution = new Vector();
                for (var idx = 0; idx < n; idx++)
                    zeroSolution.Add(0);
                return zeroSolution;
            }

            // Exception
            for (var k = i + 1; k < n; k++)
            {
                var factor = augmented[k, i] / augmented[i, i];
                for (var j = i; j <= n; j++)
                    augmented[k, j] -= factor * augmented[i, j];
            }
        }

        // Backward elimination
        var solution = new Vector();
        for (var i = 0; i < n; i++)
            solution.Add(0);

        for (var i = n - 1; i >= 0; i--)
        {
            solution[i] = augmented[i, n];
            for (var j = i + 1; j < n; j++)
                solution[i] -= augmented[i, j] * solution[j];
            solution[i] /= augmented[i, i];
        }

        return solution;
    }
}