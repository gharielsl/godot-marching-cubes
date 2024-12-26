using Godot;

public partial class World : Node3D
{
	private ClientPlayer _player;
	private Node3D _players;
	private Node3D _chunks;
	public override void _Ready()
	{
		base._Ready();
		_players = GetNode<Node3D>("Players");
		_chunks = GetNode<Node3D>("Chunks");
	}
	public void Connected(Player player, Player[] existing)
	{
		PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://player/client_player.tscn");
		foreach (Player existingPlayer in existing)
		{
			ClientPlayer existingPlayerScene = playerScene.Instantiate<ClientPlayer>();
			existingPlayerScene.NetworkId = existingPlayer.NetworkId;
			_players.AddChild(existingPlayerScene);
		}
		_player = playerScene.Instantiate<ClientPlayer>();
		_player.NetworkId = player.NetworkId;
		_players.AddChild(_player);
	}
	public void PlayerJoined(Player player)
	{
		if (player.NetworkId == Multiplayer.GetUniqueId())
		{
			return;
		}
		ClientPlayer scenePlayer = ResourceLoader.Load<PackedScene>("res://player/client_player.tscn").Instantiate<ClientPlayer>();
		scenePlayer.NetworkId = player.NetworkId;
		_players.AddChild(scenePlayer);
	}
	public void PlayerLeft(Player player)
	{
		for (int i = 0; i < _players.GetChildren().Count; i++)
		{
			if ((_players.GetChildren()[i] as ClientPlayer).NetworkId == player.NetworkId)
			{
				_players.RemoveChild(_players.GetChildren()[i]);
			}
		}
	}
}
