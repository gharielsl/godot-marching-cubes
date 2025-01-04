using Godot;

public class HighBeachEdgeBiome : ForestBiome
{
    public static new readonly HighBeachEdgeBiome Instance = new();
    public HighBeachEdgeBiome()
    {
        BaseHeight = HighBeachBiome.Instance.BaseHeight + 2;
        HeightVariation = 4;
    }
    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        Voxel voxel = base.GetVoxel(chunk, data, seed, sample, x, y, z);
        //if (y < BaseHeight - 1 && voxel == GrassVoxel.Instance)
        //{
        //    voxel = sample.F < 0.5 ? StoneVoxel.Instance : MossStoneVoxel.Instance;
        //}
        if (sample.C < -0.5)
        {
            voxel = AirVoxel.Instance;
        }
        return voxel;
    }
}