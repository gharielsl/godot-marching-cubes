public class RiverBiome : Biome
{
    public static readonly RiverBiome Instance = new();
    public RiverBiome(): base(SeaLevelHeight - 2, 2)
    {

    }
    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        Voxel voxel = AirVoxel.Instance;
        if (y < sample.H1 - 4)
        {
            voxel = StoneVoxel.Instance;
        }
        else if (y <= (int)sample.H1)
        {
            voxel = DirtVoxel.Instance;
        }
        if (y == (int)sample.H2 && y < (int)sample.H1 + 2)
        {
            voxel = StoneVoxel.Instance;
            if (y > 0)
            {
                data[x, y - 1, z] = voxel;
                if (x > 0) data[x - 1, y - 1, z] = voxel;
                if (z > 0) data[x, y - 1, z - 1] = voxel;
            }
        }
        if (y <= SeaLevelHeight && voxel == AirVoxel.Instance)
        {
            voxel = WaterVoxel.Instance;
        }
        return voxel;
    }
}