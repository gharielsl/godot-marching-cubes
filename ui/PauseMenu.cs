using Godot;
using System;

public partial class PauseMenu : Control
{
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Input.IsActionJustPressed("pause"))
		{
			if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
			{
				return;
			}
			Visible = !Visible;
			if (Visible)
			{
				Pause();
			}
			else
			{
				Continue();
			}
		}
	}
	private void Pause()
	{
		Global.Player.MouseModeBeforePause = Input.MouseMode;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = Global.IsHost;
	}
	private void LeaveGame()
	{
		GetTree().Paused = false;
		Global.Network.CloseServer();
		Global.GlobalNode.ChangeSceneToFile("res://ui/menu.tscn");
	}
	private void Continue()
	{
		GetTree().Paused = false;
		Visible = false;
		Global.Player.UnPause();
	}
}
