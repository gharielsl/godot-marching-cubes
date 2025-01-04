using Godot;
public class ForestBiome : Biome
{
    public static readonly ForestBiome Instance = new();
    public ForestBiome()
    {
        BaseHeight = BeachBiome.Instance.BaseHeight + 3;
        HeightVariation = 8;
    }
    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
    {
        Voxel voxel = base.GetVoxel(chunk, data, seed, sample, x, y, z);
        if (voxel == ObsidianVoxel.Instance)
        {
            return voxel;
        }

        sample.H1 -= sample.B * 10;
        sample.H1 -= sample.C * 2;
        //h1 += c * 2;
        //h2 += c * 4;

        Voxel stone = sample.F < 0.5 ? StoneVoxel.Instance : MossStoneVoxel.Instance;

        if (y == (int)sample.H1)
        {
            voxel = GrassVoxel.Instance;
        }
        else if (y < sample.H1 - 3)
        {
            voxel = stone;
        }
        else if (y < (int)sample.H1)
        {
            voxel = DirtVoxel.Instance;
        }
        if (y == (int)sample.H2 && y < (int)sample.H1 + 2)
        {
            voxel = stone;
            Propagate(data, x, y, z, voxel);
        }
        //if (h2 > h1 && y < h1 + 6)
        //{
        //    voxel = stone;
        //    Propagate(data, x, y, z, voxel);
        //} 
        //else if (h2 + 3 > h1 && y > h1 + 6 && y < h1 + 9)
        //{
        //    voxel = GrassVoxel.Instance;
        //    Propagate(data, x, y, z, voxel);
        //}
        return voxel;
    }

    private static void Propagate(Voxel[,,] data, int x, int y, int z, Voxel voxel)
    {
        if (y > 0)
        {
            data[x, y - 1, z] = voxel;
            if (x > 0) data[x - 1, y - 1, z] = voxel;
            if (z > 0) data[x, y - 1, z - 1] = voxel;
        }
    }
}