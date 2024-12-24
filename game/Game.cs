using Godot;

public partial class Game : Node3D
{
	protected Network _network;
	private World _world;
	public void Connect(string address, short port)
	{
		_network.Game = this;
		_network.Peer = new ENetMultiplayerPeer();
		_network.Peer.CreateClient(address, port);
		_network.Peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
	}
	public override void _Ready()
	{
		base._Ready();
		if (this is Singleplayer)
		{
			_network = GetNode<Network>("Server/Network");
		}
		else
		{
			_network = GetNode<Network>("Network");
		}
		_world = GetNode<World>("World");
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
