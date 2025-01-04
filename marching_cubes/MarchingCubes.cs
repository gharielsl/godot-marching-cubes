using Godot;
using System.Collections.Generic;

public class MarchingCubes
{
    public static readonly bool SmoothNormals = true;
    public static readonly float TerrainSurface = 0.5f;

    public static int GetConfigIndex(float[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > TerrainSurface)
            {
                configurationIndex |= 1 << i;
            }
        }
        return configurationIndex;
    }

    public static void MarchCube(
        Vector3I position,
        Voxel[,,] map,
        List<Vector3> positions,
        List<int> indices,
        Dictionary<Vector3, int> uniquePositions,
        List<Vector3> transparentPositions,
        List<int> transparentIndices,
        Dictionary<Vector3, int> transparentUniquePositions)
    {
        float[] cube = new float[8];
        float[] transparentCube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3I voxelPosition = position + CubeTables.CornerTable[i];
            Voxel voxel = map[voxelPosition.X, voxelPosition.Y, voxelPosition.Z];
            if (voxel == null)
            {
                return;
            }
            if (voxel.IsTransparent)
            {
                cube[i] = AirVoxel.DENSITY;
                transparentCube[i] = voxel.Density;
            }
            else
            {
                transparentCube[i] = AirVoxel.DENSITY;
                cube[i] = voxel.Density;
            }
        }
        int configIndex = GetConfigIndex(cube);
        int configIndexTransparent = GetConfigIndex(transparentCube);
        IterateEdges(configIndex, position, positions, indices, uniquePositions, false, cube);
        IterateEdges(configIndexTransparent, position, transparentPositions, transparentIndices, transparentUniquePositions, true, transparentCube);
    }
    private static void IterateEdges(
        int configIndex,
        Vector3I position,
        List<Vector3> positions,
        List<int> indices,
        Dictionary<Vector3, int> uniquePositions,
        bool isTransparent,
        float[] cube)
    {
        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }
        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = CubeTables.TriTable[configIndex][edgeIndex];
                if (index == -1)
                {
                    return;
                }
                Vector3 vert1 = position + CubeTables.CornerTable[CubeTables.EdgeTable[index][0]];
                Vector3 vert2 = position + CubeTables.CornerTable[CubeTables.EdgeTable[index][1]];

                float vert1Sample = cube[CubeTables.EdgeTable[index][0]];
                float vert2Sample = cube[CubeTables.EdgeTable[index][1]];
                float difference = vert2Sample - vert1Sample;
                if (difference == 0) difference = TerrainSurface;
                else difference = (TerrainSurface - vert1Sample) / difference;
                Vector3 vertPosition = vert1 + (vert2 - vert1) * difference;

                if (isTransparent)
                {
                    vertPosition.Y -= 1;
                }
                if (SmoothNormals)
                {
                    if (uniquePositions.ContainsKey(vertPosition))
                    {
                        indices.Add(uniquePositions[vertPosition]);
                    }
                    else
                    {
                        positions.Add(vertPosition);
                        indices.Add(positions.Count - 1);
                        uniquePositions.Add(vertPosition, positions.Count - 1);
                    }
                }
                else
                {
                    positions.Add(vertPosition);
                    indices.Add(positions.Count - 1);
                }
                edgeIndex++;
            }
        }
    }
}