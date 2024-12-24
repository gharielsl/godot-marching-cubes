using Godot;

public partial class Menu : Node3D
{
	[Export]
	public PackedScene SingleplayerScene;
	[Export]
	public PackedScene MultiplayerScene;
	private void SingleplayerClick()
	{
		GetTree().ChangeSceneToPacked(SingleplayerScene);
	}
	private void JoinClick()
	{

	}
	private void OptionsClick()
	{
		
	}
	private void LeaveClick()
	{
		GetTree().Quit();
	}
}
