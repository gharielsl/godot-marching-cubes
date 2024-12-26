using Godot;

public partial class NetworkNode : Node
{
	private Server _server;
	private Game _game;
	private ENetMultiplayerPeer _peer;
	public Server Server
	{
		get { return _server; }
		set { _server = value; }
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void ClientPlayerConnected(Godot.Collections.Dictionary playerDict, Godot.Collections.Array<Godot.Collections.Dictionary> existing)
    {
		Player[] existingArray = new Player[existing.Count];
		for (int i = 0; i < existingArray.Length; i++)
		{
			existingArray[i] = new Player(existing[i]);
		}
		Player player = new Player(playerDict);
		if (_game != null)
		{
			_game.Connected(player, existingArray);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void PlayerJoined(Godot.Collections.Dictionary playerDict)
	{
		Player player = new Player(playerDict);
		if (_game != null)
		{
			_game.PlayerJoined(player);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void PlayerLeft(Godot.Collections.Dictionary playerDict)
	{
		Player player = new Player(playerDict);
		if (_game != null)
		{
			_game.PlayerLeft(player);
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
}
