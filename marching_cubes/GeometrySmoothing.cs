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
}