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
    public virtual Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, double h1, double h2, int x, int y, int z)
    {
        return AirVoxel.Instance;
    }
}