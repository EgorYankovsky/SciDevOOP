using System.Reflection.Metadata.Ecma335;
using SciDevOOP.Functionals.IntegrationNormTools.Meshes;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools.Splines;

class LinearInterpolatingSpline : IParametricSpline
{
    class InternalLinearInterpolatingSpline : ISpline
    {
        private IMesh? _mesh;
     
        public IVector? q;

        public IVector? mesh;

        public double Value(IVector point)
        {
            if (mesh is null) throw new NullReferenceException("Parameter mesh set as null value.");
            if (q is null) throw new NullReferenceException("Parameter q set as null value.");
            if (point.Count != (mesh.Count / q.Count)) throw new ArgumentException("Point and mesh dimenstions aren't equal.");
            var dimension = mesh.Count / q.Count;
            switch (dimension)
            {
                case 1: _mesh = new OneDimMesh(); break;
                case > 1: _mesh = new MultiDimMesh(); mesh.Add(dimension); break;
                default: throw new ArgumentException($"Congratulations, seems like you've reached exception, that must be never raised. Dimension={dimension}");
            }
            _mesh.Bind(mesh);
            (var indexes, var points) = _mesh.FindElem(point);

            // ! Attention. Following part of code works only for 1-dim spline.
            if (dimension != 1) throw new NotImplementedException("Unfortunately, program can't yet handle with multi-dim spline");
            return indexes is not null && points is not null ? q[indexes[0]] * ((points[1][0] - point[0]) / (points[1][0] - points[0][0])) + q[indexes[1]] * ((point[0] - points[0][0]) / (points[1][0] - points[0][0])) : 0;
        }

    }

    ISpline IParametricSpline.Bind(IVector y, IVector mesh)
    {
        if (mesh.Count % y.Count != 0) throw new ArgumentException("Wrong parameters during Spline binding.");
        return new InternalLinearInterpolatingSpline
        {
            q = y,
            mesh = mesh
        };
    }
}