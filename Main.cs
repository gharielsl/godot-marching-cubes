using Godot;
public partial class Main : Node
{
	Node3D _world;
	public override void _Ready()
	{
		base._Ready();
		Voxel.RegisterVoxel(AirVoxel.Instance);
		Voxel.RegisterVoxel(DirtVoxel.Instance);
		Voxel.RegisterVoxel(GrassVoxel.Instance);
		_world = GetNode<Node3D>("World");
		Global.GlobalNode.Main = this;
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		_world.Visible = !Global.IsInGame;
	}
}
