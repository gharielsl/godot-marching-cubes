using Godot;
using System;

public partial class Server : Node
{
	public Action OnReady;
	private NetworkNode _network;
	private short _port;
	private WorldData _world;

	private void Listen(short port)
	{
		_port = port;
		if (_network.Peer == null)
        {
			_network.Peer = new ENetMultiplayerPeer();
			Error error = _network.Peer.CreateServer(port);
			if (error != Error.Ok)
			{
				GD.PrintErr(error);
			}
			Multiplayer.MultiplayerPeer = _network.Peer;
			_network.Peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		}
		GD.Print("Server opened on: ", port);
	}
	public void PeerConnected(long id)
	{
		GD.Print("Server peer connected: ", id);
		PlayerData player = new()
		{
			NetworkId = (int)id
		};
		Godot.Collections.Array<Godot.Collections.Dictionary> existingPlayers = new();
		existingPlayers.Resize(_world.Players.Count);
		int i = 0;
		foreach (PlayerData playerData in _world.Players.Values)
        {
			existingPlayers[i] = playerData.Dictionary;
			i++;
		}
		_network.Rpc(nameof(_network.PlayerJoined), player.Dictionary);
		_network.RpcId(id, nameof(_network.ClientPlayerConnected), player.Dictionary, existingPlayers);
		_world.PlayerJoined(player);
	}
	private void PeerDisconnected(long id)
	{
		GD.Print("Server peer disconnected: ", id);
		PlayerData player = _world.Players[(int)id];
		_world.PlayerLeft(player);
		_network.Rpc(nameof(_network.PlayerLeft), player.Dictionary);
	}
	public void PlayerUpdated(PlayerData player)
    {
		_world.PlayerUpdated(player);
	}
	public void PlaceVoxels(Vector3I[] positions, int[] voxels)
    {
		_world.PlaceVoxels(positions, voxels);
	}
	public override void _Ready()
	{
		base._Ready();
		_world = new WorldData(this);
		_network = GetTree().Root.GetNode<NetworkNode>("NetworkNode");
		_network.Server = this;
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Listen(5000);
		if (OnReady != null)
		{
			OnReady();
		}
	}
    public override void _Process(double delta)
    {
        base._Process(delta);
		_world.Update(delta);
    }
    public override void _ExitTree()
    {
        base._ExitTree();
		foreach (int peer in _world.Players.Keys)
        {
			if (peer != 1)
            {
				_network.Peer.DisconnectPeer(peer, true);
			}
		}
    }
    public short Port
	{
		get { return _port; }
	}
	public NetworkNode Network
    {
		get { return _network; }
    }
}
