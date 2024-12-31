using Godot;

public partial class Menu : Control
{
	private LineEdit _address;
	private LineEdit _port;
	private Button _join;
	private void SingleplayerClick()
	{
		Global.IsHost = true;
		Global.GlobalNode.ChangeSceneToFile("res://game/game.tscn");
	}
	private async void JoinClick()
	{
		if (_address.Text.Length < 4)
		{
			return;
		}
		_join.Text = "Connecting...";
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		try
		{
			Global.MultiplayerPort = short.Parse(_port.Text);
			Global.MultiplayerAddress = _address.Text;
			Global.IsHost = false;
			Global.GlobalNode.ChangeSceneToFile("res://game/game.tscn");
		}
		catch { }
	}
	private void AddressOrPortChanged(string newText)
	{
		if (_join.Text == "Failed to connect")
		{
			_join.Text = "Join";
		}
	}
	private void OptionsClick()
	{
		Global.GlobalNode.ChangeSceneToFile("res://ui/options.tscn");
	}
	private void LeaveClick()
	{
		GetTree().Quit();
	}
	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Visible;
		_address = GetNode<LineEdit>("Menu/JoinContainer/Address");
		_port = GetNode<LineEdit>("Menu/JoinContainer/Port");
		_join = GetNode<Button>("Menu/JoinContainer/Join");
		if (Global.FailedToConnect)
		{
			_join.Text = "Failed to connect";
			_address.Text = Global.MultiplayerAddress;
			_port.Text = Global.MultiplayerPort + "";
		}
		Global.FailedToConnect = false;
	}
}
