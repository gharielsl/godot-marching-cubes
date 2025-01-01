using Godot;
using System;

public partial class Options : Control
{
	private Label _renderDistanceText;
	private HSlider _renderDistanceRange;
	private Label _meshRenderDistanceText;
	private HSlider _meshRenderDistanceRange;
	private Label _meshCountText;
	private HSlider _meshCountRange;
	public override void _Ready()
	{
		base._Ready();
		_renderDistanceRange = GetNode<HSlider>("Container/RenderContainer/RenderDistance");
		_renderDistanceText = GetNode<Label>("Container/RenderContainer/RenderDistanceText");

		_meshRenderDistanceRange = GetNode<HSlider>("Container/MeshDistanceContainer/MeshDistance");
		_meshRenderDistanceText = GetNode<Label>("Container/MeshDistanceContainer/MeshDistanceText");

		_meshCountRange = GetNode<HSlider>("Container/MeshCountContainer/MeshCount");
		_meshCountText = GetNode<Label>("Container/MeshCountContainer/MeshCountText");

		_renderDistanceRange.Value = WorldData.ChunkRenderDistance;
		_meshRenderDistanceRange.Value = Chunk.SurfaceMeshRenderDistance;
		_meshCountRange.Value = Chunk.SurfaceMeshCount;
		RenderDistanceChanged((float)_renderDistanceRange.Value);
		MeshDistanceChange((float)_meshRenderDistanceRange.Value);
		MeshCountChange((float)_meshCountRange.Value);
	}
	private void RenderDistanceChanged(float d)
	{
		_renderDistanceText.Text = $"Render distance: {d} Chunks";
		WorldData.ChunkRenderDistance = (int)d;
	}
	private void MeshDistanceChange(float d)
	{
		_meshRenderDistanceText.Text = $"Mesh render distance: {d} Chunks";
		Chunk.SurfaceMeshRenderDistance = (int)d;
	}
	private void MeshCountChange(float d)
	{
		_meshCountText.Text = $"Surface mesh count: {d}";
		Chunk.SurfaceMeshCount = (int)d;
	}
	private void MeshCountChangeEvent(float d)
	{
		MeshCountChange(d);
		if (Global.IsInGame)
		{
			Global.Game.RegenerateChunks();
		}
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
