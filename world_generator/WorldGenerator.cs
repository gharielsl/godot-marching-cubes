using Godot;
using System;

public class WorldGenerator
{
    public static Voxel[,,] Generate(ChunkData chunk, int seed)
    {
        FastNoise noise = new FastNoise(seed);
        Voxel[,,] data = new Voxel[ChunkData.ChunkSize, WorldData.WorldHeight, ChunkData.ChunkSize];
        double noiseScale = 4;
        for (int x = 0; x < ChunkData.ChunkSize; x++)
        {
            for (int y = 0; y < WorldData.WorldHeight; y++)
            {
                for (int z = 0; z < ChunkData.ChunkSize; z++)
                {
                    if (y == 0)
                    {
                        data[x, y, z] = StoneVoxel.Instance;
                        continue;
                    }
                    Biome blendedBiome = GetBlendedBiome(x + ChunkData.ChunkSize * chunk.X, 0, z + ChunkData.ChunkSize * chunk.Z, noise, out Biome biome, out double biomeValue);
                    RandomNumberGenerator random = new();
                    random.Seed = (ulong)(seed + Mathf.Abs(x * y + 0.2777183 * Mathf.Sin(z - seed)));
                    float f = random.Randf();
                    double h1 = blendedBiome.BaseHeight + blendedBiome.HeightVariation * noise.GetPerlin((chunk.X * ChunkData.ChunkSize + x) * noiseScale, (chunk.Z * ChunkData.ChunkSize + z) * noiseScale);
                    double h2 = blendedBiome.BaseHeight - 2 + 12 * noise.GetPerlin((chunk.X * ChunkData.ChunkSize + x) * 16, (chunk.Z * ChunkData.ChunkSize + z) * 16);
                    double c = noise.GetPerlin((chunk.X * ChunkData.ChunkSize + x) * 8, y * 8, (chunk.Z * ChunkData.ChunkSize + z) * 8);
                    NoiseSample sample = new()
                    {
                        H1 = h1,
                        H2 = h2,
                        C = c,
                        F = f,
                        B = biomeValue
                    };
                    data[x, y, z] = biome.GetVoxel(chunk, data, seed, sample, x, y, z);
                }
            }
        }
        return data;
    }
    public static Biome GetBiome(double noise)
    {
        if (noise < -0.75)
        {
            return ForestBiome.Instance;
        }
        if (noise < -0.5)
        {
            return HighForestBiome.Instance;
        }
        if (noise < -0.3)
        {
            return HighBeachBiome.Instance;
        }
        if (noise < -0.25)
        {
            return HighBeachEdgeBiome.Instance;
        }
        if (noise < -0.125)
        {
            return ForestBiome.Instance;
        }
        if (noise < 0)
        {
            return BeachBiome.Instance;
        }
        if (noise < 0.25)
        {
            return RiverBiome.Instance;
        }
        if (noise < 0.35)
        {
            return BeachBiome.Instance;
        }
        if (noise < 0.5)
        {
            return ForestBiome.Instance;
        }
        if (noise < 0.55)
        {
            return HighBeachEdgeBiome.Instance;
        }
        if (noise < 0.65)
        {
            return HighBeachBiome.Instance;
        }
        return CavesBiome.Instance;
        // forest
        // high forest
        // high beach
        // high beach edge
        // forest
        // beach
        // river
        // beach
        // forest
        // high beach edge
        // high beach
        // high forest
        // forest
    }
    public static Biome GetBlendedBiome(int x, int y, int z, FastNoise noise, out Biome currentBiome, out double biomeNoise)
    {
        Biome blendedBiome = new(0, 0);
        Biome oCurrentBiome = blendedBiome;
        double totalBiomeNoise = 0;
        for (int i = -Biome.BiomeBlendSize; i <= Biome.BiomeBlendSize; i++)
        {
            for (int k = -Biome.BiomeBlendSize; k <= Biome.BiomeBlendSize; k++)
            {
                double biomeValue = noise.GetPerlin(x / 2d + i / 2d, z / 2d + k / 2d);
                totalBiomeNoise += biomeValue;
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
        totalBiomeNoise /= totalSamples;
        biomeNoise = totalBiomeNoise;
        return blendedBiome;
    }
}