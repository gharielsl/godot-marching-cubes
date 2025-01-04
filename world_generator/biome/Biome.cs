using Godot;

public class Biome
{
    public static readonly int BiomeBlendSize = 2;
    public static readonly float NormalBaseHeight = 12;
    public static readonly float NormalHeightVariation = 6;
    public static readonly float SeaLevelHeight = 10;
    public float BaseHeight = NormalBaseHeight;
    public float HeightVariation = NormalHeightVariation;
    public Biome(float baseHeight, float heightVariation)
    {
        BaseHeight = baseHeight;
        HeightVariation = heightVariation;
    }
    public Biome() { }
    public virtual Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        if (this is BeachBiome && this is not HighBeachBiome)
        {
            return AirVoxel.Instance;
        }
        Voxel voxel = AirVoxel.Instance;
        if (sample.C > 0.6 && y < sample.H1 + 2)
        {
            voxel = ObsidianVoxel.Instance;
        }
        return voxel;
    }
}