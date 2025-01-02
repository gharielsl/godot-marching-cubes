using Godot;
using System.Collections.Generic;

public partial class Voxel
{
	public static readonly Dictionary<int, Voxel> Voxels = new();
	public static void RegisterVoxel(Voxel voxel)
	{
		GD.Print(voxel, voxel.Id);
		Voxels.Add(voxel.Id, voxel);
	}
	private readonly int _id;
	private readonly float _density;
	private readonly bool _transparent;
	public Voxel(int id, float density, bool transparent)
	{
		_id = id;
		_density = density;
		_transparent = transparent;
	}
	public int Id 
	{ 
		get { return _id; }
	}
	public float Density
	{
		get { return _density; }
	}
	public bool IsTransparent
    {
		get { return _transparent; }
    }
}
