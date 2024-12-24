using Godot;

public partial class Network : Node
{
	private Server _server;
	private Game _game;
	private ENetMultiplayerPeer _peer;
	public Server Server
	{
		get { return _server; }
		set { _server = value; }
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
