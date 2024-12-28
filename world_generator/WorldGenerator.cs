public class WorldGenerator
{
    public static Voxel[,,] Generate(int chunkX, int chunkZ, int seed)
    {
        FastNoise noise = new FastNoise(seed);
        Voxel[,,] data = new Voxel[ChunkData.ChunkSize, WorldData.WorldHeight, ChunkData.ChunkSize];
        double baseHeight = 12;
        double heightVariation = 6;
        double noiseScale = 8;
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < WorldData.WorldHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    data[x, y, z] = AirVoxel.Instance;
                    double n = baseHeight + heightVariation * noise.GetPerlin((chunkX * ChunkData.ChunkSize + x) * noiseScale, (chunkZ * ChunkData.ChunkSize + z) * noiseScale);
                    if (n < y)
                    {
                        data[x, y, z] = DirtVoxel.Instance;
                    }
                }
            }
        }
        return data;
    }
}