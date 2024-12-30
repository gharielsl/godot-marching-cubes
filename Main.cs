using Godot;
public partial class Main : Node
{
	public override void _Ready()
	{
		base._Ready();
		Voxel.RegisterVoxel(AirVoxel.Instance);
		Voxel.RegisterVoxel(DirtVoxel.Instance);
		Voxel.RegisterVoxel(GrassVoxel.Instance);
		Global.GlobalNode.Main = this;
	}
}
