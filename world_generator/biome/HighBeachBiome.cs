public class HighBeachBiome : BeachBiome
{
    public static new readonly HighBeachBiome Instance = new();
    public HighBeachBiome()
    {
        BaseHeight = ForestBiome.Instance.BaseHeight + 10;
        HeightVariation = 1;
    }
    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        Voxel voxel = base.GetVoxel(chunk, data, seed, sample, x, y, z);
        if (sample.C < -0.5)
        {
            voxel = AirVoxel.Instance;
        }
        return voxel;
    }
}