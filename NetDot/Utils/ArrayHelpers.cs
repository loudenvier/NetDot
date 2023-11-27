using System;
using System.Collections.Generic;

namespace NetDot.Utils
{
    public static class ArrayHelpers
    {
        public static IList<int[]> CollectIndices(this Array array) {
            var result = new List<int[]>();
            VisitItems(array, result.Add);
            return result;
        }

        public static void VisitItems(this Array array, Action<object, int[]> visitor) 
            => VisitItems(array, (int[] idx) => {
                var value = array.GetValue(idx);
                visitor(value, idx);
            });

        public static void VisitItems(this Array array, Action<int[]> visitor, int[]? indices = null) {
            indices ??= Array.Empty<int>();
            int dimension = indices.Length;
            int[] newIndices = new int[dimension + 1];
            for (int i = 0; i < dimension; i++)
                newIndices[i] = indices[i];

            for (int i = array.GetLowerBound(dimension); i <= array.GetUpperBound(dimension); i++) {
                newIndices[dimension] = i;
                if (newIndices.Length == array.Rank) // top-level item
                    visitor((int[])newIndices.Clone());
                else
                    VisitItems(array, visitor, newIndices);
            }
        }

    }
}
