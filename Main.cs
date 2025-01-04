using Godot;
public partial class Main : Node
{
	Node3D _world;
	private static void RegisterSurfaceMehses()
	{
		Mesh grass = ResourceLoader.Load<ArrayMesh>("res://voxel/surface_mesh/grass.res");
		ShaderMaterial grassMat = ResourceLoader.Load<ShaderMaterial>("res://shaders/surface_mesh.tres");
		grassMat.SetShaderParameter("color", ResourceLoader.Load<Texture2D>("res://voxel/surface_mesh/grass.png"));
		grassMat.SetShaderParameter("animated", true);
		grassMat.SetShaderParameter("glow", false);
		grassMat.ResourceLocalToScene = true;
		SurfaceMesh.SurfaceMeshes.Add(GrassVoxel.ID, new SurfaceMesh(grass, grassMat, 0.5f, 20));

		Mesh crystal = ResourceLoader.Load<ArrayMesh>("res://voxel/surface_mesh/crystal.res");
		ShaderMaterial crystalMat = ResourceLoader.Load<ShaderMaterial>("res://shaders/surface_mesh.tres").Duplicate(true) as ShaderMaterial;
		crystalMat.ResourceLocalToScene = true;
		crystalMat.SetShaderParameter("color", ResourceLoader.Load<Texture2D>("res://voxel/surface_mesh/crystal.jpg"));
		crystalMat.SetShaderParameter("animated", false);
		crystalMat.SetShaderParameter("glow", true);
		SurfaceMesh.SurfaceMeshes.Add(ObsidianVoxel.ID, new SurfaceMesh(crystal, crystalMat, -1, 1));
	}
	public override void _Ready()
	{
		base._Ready();
		Voxel.RegisterVoxel(AirVoxel.Instance);
		Voxel.RegisterVoxel(DirtVoxel.Instance);
		Voxel.RegisterVoxel(GrassVoxel.Instance);
		Voxel.RegisterVoxel(StoneVoxel.Instance);
		Voxel.RegisterVoxel(SandVoxel.Instance);
		Voxel.RegisterVoxel(WaterVoxel.Instance);
		Voxel.RegisterVoxel(MossStoneVoxel.Instance);
		Voxel.RegisterVoxel(ObsidianVoxel.Instance);
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
