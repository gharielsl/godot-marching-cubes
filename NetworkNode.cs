using Godot;

public partial class NetworkNode : Node
{
	private Server _server;
	private Game _game;
	private ENetMultiplayerPeer _peer;
    public override void _Ready()
    {
        base._Ready();
		Global.Network = this;
    }
    public Server Server
	{
		get { return _server; }
		set { _server = value; }
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void ClientPlayerConnected(Godot.Collections.Dictionary playerDict, Godot.Collections.Array<Godot.Collections.Dictionary> existing)
    {
		PlayerData[] existingArray = new PlayerData[existing.Count];
		for (int i = 0; i < existingArray.Length; i++)
		{
			existingArray[i] = new PlayerData(existing[i]);
		}
		PlayerData player = new PlayerData(playerDict);
		if (_game != null)
		{
			_game.Connected(player, existingArray);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void PlayerUpdated(Godot.Collections.Dictionary playerDict)
    {
		PlayerData playerData = new(playerDict);
		if (_game != null)
        {
			_game.PlayerUpdated(playerData);
        }
		if (_server != null)
        {
			_server.PlayerUpdated(playerData);
		}
    }
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void PlayerJoined(Godot.Collections.Dictionary playerDict)
	{
		PlayerData player = new(playerDict);
		if (_game != null)
		{
			_game.PlayerJoined(player);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void PlayerLeft(Godot.Collections.Dictionary playerDict)
	{
		PlayerData player = new(playerDict);
		if (_game != null)
		{
			_game.PlayerLeft(player);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void ChunkLoaded(int x, int z, int[] chunkData)
    {
		if (_game != null)
        {
			_game.ChunkLoaded(x, z, chunkData);
        }
    }
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void ChunkUnloaded(int x, int z)
	{
		if (_game != null)
        {
			_game.ChunkUnloaded(x, z);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void WorldTimeUpdated(double worldTime)
    {
		if (_game != null)
        {
			_game.WorldTimeUpdated(worldTime);
		}
    }
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void PlaceVoxels(Godot.Collections.Array<Vector3I> positions, int[] voxels)
    {
		Vector3I[] positionsArray = new Vector3I[positions.Count];
		for (int i = 0; i < positions.Count; i++)
        {
			positionsArray[i] = positions[i];
        }
		if (Game != null)
        {
			Game.PlaceVoxels(positionsArray, voxels);
        }
		if (Server != null)
        {
			Server.PlaceVoxels(positionsArray, voxels);
		}
    }
	public Game Game
	{
		get { return _game; }
		set { _game = value; }
	}
	public ENetMultiplayerPeer Peer
	{
		get { return _peer; }
		set { _peer = value; }
	}
	public void CloseServer()
    {
		_peer.Close();
		_peer.Dispose();
		Multiplayer.Dispose();
		GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface());
		_peer = null;
    }
}
