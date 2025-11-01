using SciDevOOP.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools.Meshes;

class MultiDimMesh : IMesh
{
    private IList<IVector>? _points;

    private IList<int>? _indexes;

    void IMesh.Bind(IVector parameters)
    {
        _points = [];
        var m = Convert.ToInt32(_points[^1]);
        for (int i = 0; i < m; ++i)
            _points.Add(new Vector(parameters.Skip(i * m).Take(i * m)));
        _indexes = [..Enumerable.Range(0, m + 1)];
        throw new NotImplementedException();
    }

    (IList<int>?, IList<IVector>?) IMesh.FindElem(IVector point)
    {
        throw new NotImplementedException();
    }
}