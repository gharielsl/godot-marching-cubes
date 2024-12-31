using Godot;
using System;

public partial class Hud : Control
{
	public override void _Ready()
	{
		base._Ready();
		if (GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Visible = true;
		}
	}
}
