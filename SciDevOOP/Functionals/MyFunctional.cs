using SciDevOOP.ImmutableInterfaces.Functionals;
using SciDevOOP.ImmutableInterfaces.Functions;

class MyFunctional : IFunctional
{
    public List<(double x, double y)> points;
    public double Value(IFunction function)
    {
        double sum = 0;
        foreach (var point in points)
        {
            var param = new Vector();
            param.Add(point.x);
            var s = function.Value(param) - point.y;
            sum += s * s;
        }
        return sum;
    }
}