using SciDevOOP.ImmutableInterfaces.MathematicalObjects;

namespace SciDevOOP.Mesh;

class MeshBuilderND : IMeshBuilder
{
    IMesh IMeshBuilder.Build(IList<IVector> uniques) => new MeshND(uniques);

    class MeshND : IMesh
    {
        public IList<IList<int>>? elements;
        private int dimension;
        private int[] dimensions;
        private int[] multipliers;

        public MeshND(IList<IVector> uniques)
        {
            dimension = uniques.Count;
            dimensions = new int[dimension];
            multipliers = new int[dimension];
            
            var multiplier = 1;
            for (var i = 0; i < dimension; i++)
            {
                dimensions[i] = uniques[i].Count;
                multipliers[i] = multiplier;
                multiplier *= dimensions[i];
            }
            BuildElements();
        }

        private void BuildElements()
        {
            var result = new List<IList<int>>();
            
            var totalElements = 1;
            for (int i = 0; i < dimension; i++)
                totalElements *= (dimensions[i] - 1);

            for (int elementIndex = 0; elementIndex < totalElements; elementIndex++)
            {
                var element = BuildElement(elementIndex);
                result.Add(element);
            }            
            elements = result;
        }

        private IList<int> BuildElement(int elementIndex)
        {
            int vertexCount = 1 << dimension;
            var element = new List<int>(vertexCount);
            
            var startIndices = new int[dimension];
            var tempIndex = elementIndex;
            for (var i = 0; i < dimension; i++)
            {
                startIndices[i] = tempIndex % (dimensions[i] - 1);
                tempIndex /= (dimensions[i] - 1);
            }
            
            for (var vertexMask = 0; vertexMask < vertexCount; vertexMask++)
            {
                var vertexIndex = 0;
                for (var i = 0; i < dimension; i++)
                {
                    int offset = (vertexMask >> i) & 1;
                    int coordIndex = startIndices[i] + offset;
                    vertexIndex += coordIndex * multipliers[i];
                }
                element.Add(vertexIndex);
            }
            return element;
        }

        IList<int> IMesh.GetElementsIndexes(List<int> ind)
        {
            var flatList = new List<int>();
            int n = 1;
            int sourceIndex = 0;
            for (int i = 0; i < ind.Count; ++i)
            {
                sourceIndex += ind[i] * n;
                n *= dimensions[i] - 1;
            }
            return elements![sourceIndex];
        }
    }
}