using Godot;
public class ForestBiome : Biome
{
    public static readonly ForestBiome Instance = new();

    public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, double c, double h1, double h2, int x, int y, int z)
    {
        Voxel voxel = AirVoxel.Instance;

        h1 += c * 5;
        h2 += c * 8;

        RandomNumberGenerator random = new();
        random.Seed = (ulong)(seed + Mathf.Abs(x * y + 0.2777183 * Mathf.Sin(z - seed)));
        Voxel stone = random.Randf() < 0.5 ? StoneVoxel.Instance : MossStoneVoxel.Instance;
        if (stone == MossStoneVoxel.Instance)
        {
            h1 -= c * 10;
        }
        if (random.Randf() < 0.08)
        {
            stone = DirtVoxel.Instance;
        }

        if (y == (int)h1)
        {
            voxel = GrassVoxel.Instance;
        }
        else if (y < h1 - 4)
        {
            voxel = stone;
        }
        else if (y < (int)h1)
        {
            voxel = DirtVoxel.Instance;
        }
        if (y == (int)h2 && y < (int)h1 + 2)
        {
            voxel = stone;
            Propagate(data, x, y, z, voxel);
        }
        if (h2 > h1 && y < h1 + 6)
        {
            voxel = stone;
            Propagate(data, x, y, z, voxel);
        } 
        else if (h2 + 3 > h1 && y > h1 + 6 && y < h1 + 9)
        {
            voxel = GrassVoxel.Instance;
            Propagate(data, x, y, z, voxel);
        }
        if (c > 0.6)
        {
            voxel = ObsidianVoxel.Instance;
        }
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