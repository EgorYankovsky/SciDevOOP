using SciDevOOP.ImmutableInterfaces.Functions;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools;

class LinearInterpolatingSpline : IParametricSpline
{
    class InternalLinearInterpolatingSpline : ISpline
    {
        public IVector? q;

        public IVector? mesh;

        //private Mesh? _mesh;

        public double Value(IVector point)
        {
            if (point.Count != (mesh.Count % q.Count)) throw new ArgumentException("Point and mesh dimenstions aren't equal.");
            
            //_mesh = new Mesh(mesh);
            
            throw new NotImplementedException();
        }

    }

    public ISpline Bind(IVector y, IVector mesh)
    {
        if (mesh.Count % y.Count != 0) throw new ArgumentException("Wrong parameters during Spline binding.");
        return new InternalLinearInterpolatingSpline
        {
            q = y,
            mesh = mesh
        };
    }
}