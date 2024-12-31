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
			Multiplayer.ServerDisconnected += () =>
			{
				Global.GlobalNode.ChangeSceneToFile("res://ui/disconnected.tscn");
			};
		}
		else
		{
			_network.Server.PeerConnected(1);
		}
		GD.Print("Client connected to: ", address, ":", port);
		Input.MouseMode = Input.MouseModeEnum.Captured;
		return true;
	}
	public void ChunkLoaded(int x, int z, int[] voxels)
	{
		GD.Print("Client chunk loaded at ", x, " ", z, " size: ", voxels.Length);
		_world.ChunkLoaded(x, z, voxels);
	}
	public void ChunkUnloaded(int x, int z)
	{
		GD.Print("Client chunk unloaded at ", x, " ", z);
		_world.ChunkUnloaded(x, z);
	}
	public void Connected(PlayerData playerData, PlayerData[] existing)
	{
		_world.Connected(playerData, existing);
	}
	public void PlayerJoined(PlayerData playerData)
	{
		_world.PlayerJoined(playerData);
	}
	public void PlayerLeft(PlayerData playerData)
	{
		_world.PlayerLeft(playerData);
	}
	public void PlayerUpdated(PlayerData playerData)
	{
		_world.PlayerUpdated(playerData);
	}
	public void WorldTimeUpdated(double worldTime)
	{
		_world.WorldTimeUpdated(worldTime);
	}
	public override void _Ready()
	{
		base._Ready();
		Global.IsInGame = true;
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
				Global.GlobalNode.ChangeSceneToFile("res://ui/menu.tscn");
			}
		}
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (_network.Peer != null)
		{
			_network.Peer.Poll();
			if (_world.Player != null)
			{
				_network.Rpc(nameof(_network.PlayerUpdated), new PlayerData(_world.Player).Dictionary);
			}
		}
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		Global.IsInGame = false;
	}
}
