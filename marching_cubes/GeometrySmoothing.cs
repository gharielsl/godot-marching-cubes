using Godot;
using System.Collections.Generic;

public class GeometrySmoothing
{
    public static void SmoothGeometry(List<Vector3> positions, List<int> indices)
    {
        Dictionary<int, Vector3> vertexSums = new Dictionary<int, Vector3>();
        Dictionary<int, int> vertexCounts = new Dictionary<int, int>();
        for (int i = 0; i < positions.Count; i++)
        {
            vertexSums.Add(i, new Vector3());
            vertexCounts.Add(i, 0);
        }
        for (int i = 0; i < indices.Count; i += 3)
        {
            int index0 = indices[i];
            int index1 = indices[i + 1];
            int index2 = indices[i + 2];
            AddNeighbor(index0, index1, vertexSums, vertexCounts, positions);
            AddNeighbor(index0, index2, vertexSums, vertexCounts, positions);
            AddNeighbor(index1, index2, vertexSums, vertexCounts, positions);
            AddNeighbor(index1, index0, vertexSums, vertexCounts, positions);
            AddNeighbor(index2, index0, vertexSums, vertexCounts, positions);
            AddNeighbor(index2, index1, vertexSums, vertexCounts, positions);
        }
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 sum = vertexSums[i];
            int count = vertexCounts[i];
            if (count > 0)
            {
                sum /= count;
                positions[i] = sum;
            }
        }
    }
    private static void AddNeighbor(
        int vertexIndex,
        int neighborIndex,
        Dictionary<int, Vector3> vertexSums,
        Dictionary<int, int> vertexCounts,
        List<Vector3> positions)
    {
        Vector3 vertexSum = vertexSums[vertexIndex];
        Vector3 neighborPosition = positions[neighborIndex];
        vertexSums[vertexIndex] = vertexSum + neighborPosition;
        vertexCounts[vertexIndex] = vertexCounts[vertexIndex] + 1;
    }
    public static void SubdivideGeometry(List<Vector3> positions, List<int> indices)
    {
        Dictionary<(int, int), int> midpointCache = new Dictionary<(int, int), int>();
        List<int> newIndices = new List<int>();

        for (int i = 0; i < indices.Count; i += 3)
        {
            int i0 = indices[i];
            int i1 = indices[i + 1];
            int i2 = indices[i + 2];

            int m01 = GetOrCreateMidpoint(i0, i1, positions, midpointCache);
            int m12 = GetOrCreateMidpoint(i1, i2, positions, midpointCache);
            int m20 = GetOrCreateMidpoint(i2, i0, positions, midpointCache);

            newIndices.AddRange(new[] {
                i0, m01, m20,
                m01, i1, m12,
                m20, m12, i2,
                m01, m12, m20
            });
        }
        indices.Clear();
        indices.AddRange(newIndices);
    }
    private static int GetOrCreateMidpoint(int i0, int i1, List<Vector3> positions, Dictionary<(int, int), int> cache)
    {
        (int, int) edgeKey = i0 < i1 ? (i0, i1) : (i1, i0);
        if (cache.TryGetValue(edgeKey, out int midpointIndex))
        {
            return midpointIndex;
        }
        Vector3 midpoint = (positions[i0] + positions[i1]) * 0.5f;
        midpointIndex = positions.Count;
        positions.Add(midpoint);
        cache[edgeKey] = midpointIndex;

        return midpointIndex;
    }
}