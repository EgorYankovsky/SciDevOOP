using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces;
using SciDevOOP.MathematicalObjects;

namespace SciDevOOP.Optimizators;

class MinimizerMonteCarlo : IOptimizator
{
    public double From = 0.0;
    public double To = 1.0;
    public int MaxIterations = 100_000;

    public IVector Minimize(IFunctional objective, IParametricFunction function, IVector initialParameters, IVector? minimumParameters = null, IVector? maximumParameters = null)
    {
        // 1. ������������� ��������� ������ ��� ������������� �����.
        var param = new Vector();
        var minParam = new Vector();
        
        foreach (var p in initialParameters) param.Add(p);
        foreach (var p in initialParameters) minParam.Add(p);
        
        var fun = function.Bind(param);
        var currentMin = objective.Value(fun);
        
        var rand = new Random(0);

        // 2. ��� ���� �������.
        for (int i = 0; i < MaxIterations; i++)
        {
            // ���������� ��������� ������������.
            for (int j = 0; j < param.Count; j++) param[j] = From + rand.NextDouble() * (To - From);

            // ������� �������� �����������
            var f = objective.Value(function.Bind(param));
            
            // ��������� ��������� ������� ���� �������� ����������� ������.
            if (f < currentMin)
            {
                currentMin = f;
                for (int j = 0; j < param.Count; j++) minParam[j] = param[j];
            }
        }
        return minParam;
    }
}