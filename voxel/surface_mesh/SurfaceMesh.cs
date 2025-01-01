using Godot;
using System.Collections.Generic;

public class SurfaceMesh
{
	public static readonly Dictionary<int, SurfaceMesh> SurfaceMeshes = new();
	
	private readonly Mesh _mesh;
	private readonly Material _material;
	public SurfaceMesh(Mesh mesh, Material material)
	{
		_mesh = mesh;
		_material = material;
	}
	public Mesh Mesh
	{
		get { return _mesh; }
	}
	public Material Material
	{
		get { return _material; }
	}
}
