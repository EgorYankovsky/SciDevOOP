using SciDevOOP.Functionals;
using SciDevOOP.Optimizators;
using SciDevOOP.Functions;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TransitionRules;
using SciDevOOP.Optimizators.SimulatedAnnealingTools.TemperatureChangeLaws;
using SciDevOOP.Optimizators.LevenbergMarquardtTools.Solvers;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.Functionals;

/*
 * Reading support console and TXT input.
 *  - to use console input use as: Read()
 *  - to use txt input use as: Read("file_name.txt")
 *      ! basically, it uses \\SciDevOOP\\bin\\Resources\\file_name as input folder
 *      ! also you can set here a random EXISTING! folder.
 *  
 *  Writing support console and TXT output.
 *  - to use console output use as: Write(res)
 *  - to use txt input use as: Read(res, "file_name.txt")
 *      ! basically, it uses \\SciDevOOP\\bin\\Resources\\file_name as output folder
 *      ! also you can set here a random EXISTING! folder.
 */

partial class Program
{
    static void Main(string[] args)
    {
        IList<double> answer = new List<double>();
        var allResults = new List<IVector>();
        for (int i = 0;  i < 10; i++)
        {
            IOptimizator optimizer = new MinimizerSimulatedAnnealing(new ContinuousImprovement(), new BasketFiring());
            var fun = new SplineFunction();
            var points = Read("inputSpline.txt");

            // Начальное приближение для коэффициентов c0, c1, c2
            var initial = new Vector {
                0.5, 0.0, 1.47, -0.78, 1.88,
-0.35,
0.0, 2.0, 5.0
            };
            // Границы ДЛЯ КОЭФФИЦИЕНТОВ (не для пространства интегрирования!)
            var paramLowerBound = new Vector { 0.5, 0.0, 0.47, -0.78, 0.88,
-0.35,
0.0, 0.0, 0.0 };
            var paramUpperBound = new Vector { 1.5, 1.0, 1.47, 1.78, 1.88,
1.35,
1.0, 2.0, 5.0 };

            // Границы для пространства интегрирования (отдельно!)
            var integrationLowerBound = new Vector { 0.0, 0.0 };  // min x1, min x2
            var integrationUpperBound = new Vector { 1.0, 1.0 };  // max x1, max x2

            var functional = new SimpleIntegrationNorm{                points = points
            };

            // Передаем границы для параметров, а не для интегрирования
            var res = optimizer.Minimize(functional, fun, initial, paramLowerBound, paramUpperBound);
            allResults.Add(res);
            Write(res);
        }
        answer = CalculateAverage(allResults);

        Console.WriteLine($"Средний результат: [{string.Join(", ", answer)}]");
        Write(answer);
    }
    private static IList<double> CalculateAverage(List<IVector> vectors)
    {
        if (vectors.Count == 0) return new List<double>();

        var average = new List<double>();
        int dimension = vectors[0].Count;

        for (int i = 0; i < dimension; i++)
        {
            double sum = 0;
            foreach (var vector in vectors)
            {
                sum += vector[i];
            }
            average.Add(sum / vectors.Count);
        }

        return average;
    }
}