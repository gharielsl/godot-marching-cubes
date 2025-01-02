using System;

public class WorldGenerator
{
    public static Voxel[,,] Generate(ChunkData chunk, int seed)
    {
        FastNoise noise = new FastNoise(seed);
        Voxel[,,] data = new Voxel[ChunkData.ChunkSize, WorldData.WorldHeight, ChunkData.ChunkSize];
        double noiseScale = 8;
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < WorldData.WorldHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    Biome blendedBiome = GetBlendedBiome(x + ChunkData.ChunkSize * chunk.X, 0, z + ChunkData.ChunkSize * chunk.Z, noise, out Biome biome);
                    double h1 = blendedBiome.BaseHeight + blendedBiome.HeightVariation * noise.GetPerlin((chunk.X * ChunkData.ChunkSize + x) * noiseScale, (chunk.Z * ChunkData.ChunkSize + z) * noiseScale);
                    double h2 = blendedBiome.BaseHeight - 2 + 12 * noise.GetPerlin((chunk.X * ChunkData.ChunkSize + x) * 16, (chunk.Z * ChunkData.ChunkSize + z) * 16);
                    data[x, y, z] = biome.GetVoxel(chunk, data, seed, h1, h2, x, y, z);
                }
            }
        }
        return data;
    }
    public static Biome GetBiome(double noise)
    {
        if (noise <= 0)
        {

        }
        else
        {

        }
        return ForestBiome.Instance;
    }
    public static Biome GetBlendedBiome(int x, int y, int z, FastNoise noise, out Biome currentBiome)
    {
        Biome blendedBiome = new(0, 0);
        Biome oCurrentBiome = blendedBiome;
        for (int i = -Biome.BiomeBlendSize; i <= Biome.BiomeBlendSize; i++)
        {
            for (int k = -Biome.BiomeBlendSize; k <= Biome.BiomeBlendSize; k++)
            {
                double biomeValue = 2 * noise.GetPerlin(x / 2 + i, z / 2 + k);
                Biome biome = GetBiome(biomeValue);
                blendedBiome.BaseHeight += biome.BaseHeight;
                blendedBiome.HeightVariation += biome.HeightVariation;
                if (i == 0 && k == 0)
                {
                    oCurrentBiome = biome;
                }
            }
        }
        currentBiome = oCurrentBiome;
        float totalSamples = (float)Math.Pow(Biome.BiomeBlendSize * 2 + 1, 2);
        blendedBiome.BaseHeight /= totalSamples;
        blendedBiome.HeightVariation /= totalSamples;
        return blendedBiome;
    }
}