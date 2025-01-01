using Godot;
using System;

public partial class PauseMenu : Control
{
	Control _menu;
	Options _options;
	public override void _Ready()
	{
		base._Ready();
		_menu = GetNode<Control>("Container");
		_options = GetNode<Options>("Options");
	}
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
	private void OptionsClick()
	{
		_menu.Visible = false;
		_options.Visible = true;
	}
	public void OptionsLeave()
	{
		_menu.Visible = true;
		_options.Visible = false;
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
		Global.GlobalNode.ChangeSceneToFile("res://ui/menu.tscn");
	}
	private void Continue()
	{
		GetTree().Paused = false;
		Visible = false;
		Global.Player.UnPause();
	}
}
