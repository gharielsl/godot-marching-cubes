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
}
