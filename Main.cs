using Godot;
public partial class Main : Node
{
	Node3D _world;
	private static void RegisterSurfaceMehses()
	{
		Mesh grass = ResourceLoader.Load<ArrayMesh>("res://voxel/surface_mesh/grass.res");
		ShaderMaterial grassMat = ResourceLoader.Load<ShaderMaterial>("res://shaders/surface_mesh.tres");
		grassMat.SetShaderParameter("color", ResourceLoader.Load<Texture2D>("res://voxel/surface_mesh/grass.png"));
		SurfaceMesh.SurfaceMeshes.Add(GrassVoxel.ID, new SurfaceMesh(grass, grassMat));
	}
	public override void _Ready()
	{
		base._Ready();
		Voxel.RegisterVoxel(AirVoxel.Instance);
		Voxel.RegisterVoxel(DirtVoxel.Instance);
		Voxel.RegisterVoxel(GrassVoxel.Instance);
		Voxel.RegisterVoxel(StoneVoxel.Instance);
		RegisterSurfaceMehses();
		_world = GetNode<Node3D>("World");
		Global.GlobalNode.Main = this;
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		_world.Visible = !Global.IsInGame;
	}
}
