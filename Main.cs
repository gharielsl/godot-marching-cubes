using Godot;
public partial class Main : Node
{
	public override void _Ready()
	{
		base._Ready();
		Voxel.RegisterVoxel(AirVoxel.Instance);
		Voxel.RegisterVoxel(DirtVoxel.Instance);
		Global.GlobalNode.Main = this;
	}
}
