using Godot;
using System.Collections.Generic;

public class SurfaceMesh
{
	public static readonly Dictionary<int, SurfaceMesh> SurfaceMeshes = new();
	
	private readonly Mesh _mesh;
	private readonly Material _material;
	private readonly float _normal;
	private readonly int _count;
	public SurfaceMesh(Mesh mesh, Material material, float normal, int count)
	{
		_mesh = mesh;
		_material = material;
		_normal = normal;
		_count = count;
	}
	public Mesh Mesh
	{
		get { return _mesh; }
	}
	public Material Material
	{
		get { return _material; }
	}
	public float Normal
	{
		get { return _normal; }
	}
	public int Count
	{
		get { return _count; }
	}
}
