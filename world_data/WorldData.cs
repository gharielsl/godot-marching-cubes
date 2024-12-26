using System.Collections.Generic;
using System.Threading;

public partial class WorldData
{
	public static readonly int WorldHeight = 128;
	public static readonly int ChunkRenderDistance = 16;

	private readonly Dictionary<int, Dictionary<int, ChunkData>> _chunks = new Dictionary<int, Dictionary<int, ChunkData>>();
	private readonly List<Player> _players = new List<Player>();
	private Thread _generatingThread;
	private void GeneratingLoop()
	{
		
	}
	public void PlayerJoined(Player player)
	{
		_players.Add(player);
	}
	public void PlayerLeft(Player player)
	{
		_players.Remove(player);
	}
	public Player GetPlayer(long networkId)
	{
		foreach (Player player in _players)
		{
			if (player.NetworkId == networkId)
			{
				return player;
			}
		}
		return null;
	}
	public WorldData()
	{
		_generatingThread = new Thread(GeneratingLoop);
	}
	public List<Player> Players
	{
		get { return _players; }
	}
}
