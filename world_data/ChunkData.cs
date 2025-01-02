using Godot;

public partial class ChunkData
{
	public static readonly int ChunkSize = 10;

	private readonly int _x, _z, _seed;
	private bool _disposed = false, _generating = false;
	private Voxel[,,] _data;
	public ChunkData(int x, int z, int seed)
	{
		_x = x;
		_z = z;
		_seed = seed;
	}
	public void Generate()
	{
		_data = WorldGenerator.Generate(this, _seed);
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
	public int[] CreateFlatArray()
    {
		int[] voxels = new int[ChunkSize * WorldData.WorldHeight * ChunkSize];
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < WorldData.WorldHeight; y++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    voxels[x + y * ChunkSize + z * ChunkSize * WorldData.WorldHeight] = GetVoxel(x, y, z).Id;
                }
            }
        }
        return voxels;
	}
	public void Dispose()
    {
		_disposed = true;
	}
	public bool IsDisposed
	{ 
		get { return _disposed; } 
	}
	public bool IsGenerating
    {
		get { return _generating; }
		set { _generating = value; }
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
