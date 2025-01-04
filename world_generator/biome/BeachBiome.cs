using Godot;

public class BeachBiome : Biome
{
    public static readonly BeachBiome Instance = new();
    public BeachBiome(): base(SeaLevelHeight + 1, 4)
    {

    }
    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        Voxel voxel = base.GetVoxel(chunk, data, seed, sample, x, y, z);
        Voxel stone = sample.F > 0.5 ? StoneVoxel.Instance : MossStoneVoxel.Instance;
        if (y < sample.H1 - 4)
        {
            voxel = stone;
        }
        else if (y <= (int)sample.H1)
        {
            voxel = SandVoxel.Instance;
        }
        if (y == (int)sample.H2 && y < (int)sample.H1 + 2)
        {
            voxel = stone;
            if (y > 0)
            {
                data[x, y - 1, z] = voxel;
                if (x > 0) data[x - 1, y - 1, z] = voxel;
                if (z > 0) data[x, y - 1, z - 1] = voxel;
            }
        }
        return voxel;
    }
}