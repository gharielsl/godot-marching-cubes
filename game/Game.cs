using Godot;

public partial class Game : Node3D
{
	protected NetworkNode _network;
	private World _world;
	public bool Connect(string address, short port)
	{
		_network.Game = this;
		if (_network.Server == null)
		{
			_network.Peer = new ENetMultiplayerPeer();
			Error error = _network.Peer.CreateClient(address, port);
			if (error != Error.Ok)
			{
				_network.Peer = null;
				GD.PrintErr(error);
				return false;
			}
			Multiplayer.MultiplayerPeer = _network.Peer;
			_network.Peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		}
		else
		{
			_network.Server.PeerConnected(1);
		}
		GD.Print("Client connected to: ", address, ":", port);
		Input.MouseMode = Input.MouseModeEnum.Captured;
		return true;
	}
	public void Connected(Player player, Player[] existing)
	{
		_world.Connected(player, existing);
	}
	public void PlayerJoined(Player player)
	{
		_world.PlayerJoined(player);
	}
	public void PlayerLeft(Player player)
	{
		_world.PlayerLeft(player);
	}
	public override void _Ready()
	{
		base._Ready();
		_network = GetTree().Root.GetNode<NetworkNode>("NetworkNode");
		_world = GetNode<World>("World");
		if (Global.IsHost)
		{
			_network.Server = ResourceLoader.Load<PackedScene>("res://game/server.tscn").Instantiate<Server>();
			_network.Server.OnReady = () =>
			{
				Connect("127.0.0.1", _network.Server.Port);
			};
			AddChild(_network.Server);
		}
		else
		{
			if (!Connect(Global.MultiplayerAddress, Global.MultiplayerPort))
			{
				Global.FailedToConnect = true;
				Global.GlobalNode.ChangeSceneToFile("res://main.tscn");
			}
		}
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (_network.Peer != null)
		{
			_network.Peer.Poll();
		}
	}
}
