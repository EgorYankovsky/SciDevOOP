//using System.Numerics;
using SciDevOOP.MathematicalObjects;
using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Functionals.IntegrationNormTools.Meshes;

class OneDimMesh : IMesh
{
    private IVector? _coordinates;

    private IList<int>? _nodesIndexes;

    private int FindSplineIndex(IVector point)
    {
        var indexLeft = 0;
        var indexRight = _coordinates!.Count - 1;
        while (indexRight - indexLeft > 1)
        {
            var middleIndex = (indexLeft + indexRight) / 2;
            if (point[0] == _coordinates![middleIndex])
            {
                indexLeft = middleIndex;
                break;
            }
            else if (point[0] < _coordinates[middleIndex]) indexRight = middleIndex;
            else if (point[0] > _coordinates[middleIndex]) indexLeft = middleIndex;
        }
        return indexLeft;
    }

    void IMesh.Bind(IVector parameters)
    {
        _coordinates = parameters;
        _nodesIndexes = [.. Enumerable.Range(0, _coordinates.Count)];
    }

    (IList<int>?, IList<IVector>?) IMesh.FindElem(IVector point)
    {
        if (_nodesIndexes is null) throw new NullReferenceException("Mesh parameters wasn't initialized.");
        if (point[0] < _coordinates!.First() || _coordinates!.Last() < point[0]) return (null, null);
        var leftIndex = FindSplineIndex(point);
        return (new List<int> { leftIndex, leftIndex + 1 },
                new List<IVector>
                {
                    new Vector { _coordinates![leftIndex] },
                    new Vector { _coordinates![leftIndex + 1] }
                }
                );
    }
}