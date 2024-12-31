using Godot;
using System;

public partial class Options : Control
{
	private Label _renderDistanceText;
	private HSlider _renderDistanceRange;
	public override void _Ready()
	{
		base._Ready();
		_renderDistanceRange = GetNode<HSlider>("Container/RenderContainer/RenderDistance");
		_renderDistanceText = GetNode<Label>("Container/RenderContainer/RenderDistanceText");
		_renderDistanceRange.Value = WorldData.ChunkRenderDistance;
		RenderDistanceChanged((float)_renderDistanceRange.Value);
	}
	private void RenderDistanceChanged(float d)
	{
		_renderDistanceText.Text = $"Render distance: {d} Chunks";
		WorldData.ChunkRenderDistance = (int)d;
	}
	private void Exit()
	{
		if (GetParent() is PauseMenu menu)
		{
			menu.OptionsLeave();
		}
		else
		{
			Global.GlobalNode.ChangeSceneToFile("res://ui/menu.tscn");
		}
	}
}
