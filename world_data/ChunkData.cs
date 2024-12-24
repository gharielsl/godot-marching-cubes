public partial class ChunkData
{
	public static readonly int ChunkSize = 10;

	private readonly int _x, _z, _seed;
	private Voxel[,,] _data;
	public ChunkData(int x, int z, int seed)
	{
		_x = x;
		_z = z;
		_seed = seed;
	}
	public void Generate()
	{
		_data = new Voxel[ChunkSize, WorldData.WorldHeight, ChunkSize];
	}
	public Voxel GetVoxel(int x, int y, int z)
	{
		if (_data == null)
		{
			return AirVoxel.Instance;
		}
		if (_data[x, y, z] == null)
		{
			return AirVoxel.Instance;
		}
		return _data[x, y, z];
	}
	public void SetVoxel(int x, int y, int z, Voxel voxel)
	{
		if (_data == null)
		{
			return;
		}
		if (x < 0 || x > ChunkSize || 
			y < 0 || y > WorldData.WorldHeight || 
			z < 0 || z > ChunkSize)
		{
			return;
		}
		_data[x, y, z] = voxel;
	}
	public int X 
	{ 
		get { return _x; } 
	}
	public int Z 
	{ 
		get { return _z; } 
	}
}
