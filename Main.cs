using Godot;
public partial class Main : Node
{
	public override void _Ready()
	{
		base._Ready();
		Global.GlobalNode.Main = this;
	}
}
