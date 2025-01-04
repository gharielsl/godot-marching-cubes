using Godot;
public class CavesBiome : Biome
{
	public static readonly CavesBiome Instance = new();
	public CavesBiome()
	{
		BaseHeight += 5;
	}
	public override Voxel GetVoxel(ChunkData chunk, Voxel[,,] data, int seed, NoiseSample sample, int x, int y, int z)
	{
		Voxel voxel = base.GetVoxel(chunk, data, seed, sample, x, y, z);
		if (voxel == ObsidianVoxel.Instance)
		{
			return voxel;
		}

		Voxel stone = sample.F < 0.5 ? StoneVoxel.Instance : MossStoneVoxel.Instance;

		if (sample.C < 0 && y < sample.H1 + 8)
		{
			voxel = stone;
		}
		else if (sample.C < 0 && y > sample.H1 && y < sample.H1 + 10)
		{
			voxel = GrassVoxel.Instance;
		}

		sample.H1 += sample.C * 5;

		if (stone == MossStoneVoxel.Instance)
		{
			sample.H1 -= sample.C * 10;
		}
		if (sample.F < 0.08)
		{
			stone = DirtVoxel.Instance;
		}

		if (y == (int)sample.H1)
		{
			voxel = GrassVoxel.Instance;
		}
		else if (y < sample.H1 - 4)
		{
			voxel = stone;
		}
		else if (y < (int)sample.H1)
		{
			voxel = DirtVoxel.Instance;
		}
		return voxel;
	}
}
