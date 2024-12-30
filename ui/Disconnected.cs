using Godot;
using System;

public partial class Disconnected : Control
{
	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
	private void OkPressed()
	{
		Global.GlobalNode.ChangeSceneToFile("res://ui/menu.tscn");
	}
}
