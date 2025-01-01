using Godot;
using System;
using System.Collections.Generic;

public class WorldDataUtils
{
	public static void GetChunksInRadius(WorldData world, Vector3 origin, float radius, ICollection<ChunkData> chunks)
	{
		int minX = (int)Math.Round(origin.X - radius);
		int minZ = (int)Math.Round(origin.Z - radius);
		int maxX = (int)Math.Round(origin.X + radius);
		int maxZ = (int)Math.Round(origin.Z + radius);
		for (int i = minX; i <= maxX; i++)
		{
			for (int j = minZ; j <= maxZ; j++)
			{
				if (new Vector2(origin.X, origin.Z).DistanceTo(new Vector2(i + 0.25f, j + 0.5f)) <= radius)
				{
					ChunkData chunk = world.GetChunk(i, j);
					if (chunk == null)
					{
						chunk = new ChunkData(i, j, 0);
					}
					chunks.Add(chunk);
				}
			}
		}
	}
	public static void WorldToChunk(int worldX, int worldZ, out int chunkX, out int chunkZ, out int inChunkX, out int inChunkZ)
    {
		chunkX = Mathf.FloorToInt(worldX / (double)ChunkData.ChunkSize);
		chunkZ = Mathf.FloorToInt(worldZ / (double)ChunkData.ChunkSize);
		inChunkX = (worldX % ChunkData.ChunkSize + ChunkData.ChunkSize) % ChunkData.ChunkSize;
		inChunkZ = (worldZ % ChunkData.ChunkSize + ChunkData.ChunkSize) % ChunkData.ChunkSize;
	}
	public static Vector3I ChunkToWorld(int x, int y, int z, int chunkX, int chunkZ)
    {
		return new Vector3I(
			Mathf.RoundToInt(x + chunkX * ChunkData.ChunkSize),
			Mathf.RoundToInt(y),
			Mathf.RoundToInt(z + chunkZ * ChunkData.ChunkSize));
	}
}
