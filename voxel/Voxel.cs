using System.Collections.Generic;

public partial class Voxel
{
	public static Dictionary<int, Voxel> Voxels = new Dictionary<int, Voxel>();
	public static void RegisterVoxel(Voxel voxel)
	{
		Voxels.Add(voxel.Id, voxel);
	}
	private readonly int _id;
	private readonly float _density;
	public Voxel(int id, float density)
	{
		_id = id;
		_density = density;
	}
	public int Id 
	{ 
		get { return _id; }
	}
	public float Density
	{
		get { return _density; }
	}
}
