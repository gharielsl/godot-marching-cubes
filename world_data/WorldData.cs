using Godot;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public partial class WorldData
{
	public static readonly int WorldHeight = 128;
	public static readonly int ChunkRenderDistance = 4;
	public static readonly double DayDuration = 24 * 60;

	private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, ChunkData>> _chunks = new();
	private readonly List<PlayerData> _players = new();
	private readonly HashSet<ChunkData> _previousChunks = new();
	private readonly ConcurrentQueue<ChunkData> _generatingChunks = new();
	private readonly ConcurrentQueue<ChunkData> _generatedChunks = new();
	private readonly ConcurrentDictionary<ChunkData, int[]> _generatedChunksData = new();
	private readonly Server _server;
	private Thread _chunkGenerator;
	private double _worldTime = 0;
	private void GeneratingLoop()
	{
		while (_server.IsInsideTree())
        {
			while (!_generatingChunks.IsEmpty)
            {
				_generatingChunks.TryDequeue(out ChunkData chunk);
				chunk.Generate();
				int[] voxels = chunk.CreateFlatArray();
				if (_generatedChunksData.ContainsKey(chunk))
				{
					_generatedChunksData[chunk] = voxels;
				}
				else
				{
					_generatedChunksData.TryAdd(chunk, voxels);
				}
				_generatedChunks.Enqueue(chunk);
			}
		}
	}
	public void Update(double delta)
    {
		_worldTime += delta;
		if (_worldTime > DayDuration)
        {
			_worldTime = 0;
        }
		while (!_generatedChunks.IsEmpty)
		{
			_generatedChunks.TryDequeue(out ChunkData chunk);
			_generatedChunksData.TryRemove(chunk, out int[] voxels);
			SetChunk(chunk.X, chunk.Z, chunk);
			chunk.IsGenerating = false;
			_server.Network.Rpc(nameof(_server.Network.ChunkLoaded), chunk.X, chunk.Z, voxels);
		}
		HashSet<ChunkData> currentChunks = new HashSet<ChunkData>();
		foreach (PlayerData player in _players)
		{
			WorldDataUtils.GetChunksInRadius(this, player.Position / new Vector3(ChunkData.ChunkSize, 1, ChunkData.ChunkSize), ChunkRenderDistance, currentChunks);
		}
		foreach (ChunkData chunk in currentChunks)
        {
			if (!_previousChunks.Contains(chunk))
            {
				chunk.IsGenerating = true;
				_generatingChunks.Enqueue(chunk);
				SetChunk(chunk.X, chunk.Z, chunk);
            }
        }
		foreach (ChunkData chunk in _previousChunks)
		{
			if (!currentChunks.Contains(chunk) && !chunk.IsGenerating)
			{
				SetChunk(chunk.X, chunk.Z, null);
				_server.Network.Rpc(nameof(_server.Network.ChunkUnloaded), chunk.X, chunk.Z);
			}
			else if (chunk.IsGenerating)
			{
				currentChunks.Add(chunk);
			}
		}
		_previousChunks.Clear();
		_previousChunks.UnionWith(currentChunks);
	}
	public PlayerData GetPlayer(long networkId)
	{
		foreach (PlayerData player in _players)
		{
			if (player.NetworkId == networkId)
			{
				return player;
			}
		}
		return null;
	}
	public ChunkData GetChunk(int x, int z)
    {
		if (!_chunks.ContainsKey(x))
		{
			return null;
		}
		ConcurrentDictionary<int, ChunkData> row = _chunks[x];
		if (row.ContainsKey(z))
		{
			return row[z];
		}
		return null;
	}
	public void SetChunk(int x, int z, ChunkData chunk)
	{
		if (!_chunks.ContainsKey(x))
		{
			_chunks.TryAdd(x, new ConcurrentDictionary<int, ChunkData>());
		}
		ChunkData existing = GetChunk(x, z);
		if (existing == chunk && chunk != null)
		{
			return;
		}
		if (chunk == null && existing != null)
		{
			existing.Dispose();
		}
		if (chunk == null && _chunks[x].Count <= 1)
		{
			_chunks.TryRemove(x, out ConcurrentDictionary<int, ChunkData> _);
			return;
		}
		else if (chunk == null)
		{
			_chunks[x].TryRemove(z, out ChunkData _);
			return;
		}
		_chunks[x].TryRemove(z, out ChunkData _);
		_chunks[x].TryAdd(z, chunk);
	}
	// Rpc
	public void PlayerUpdated(PlayerData playerData)
    {
		PlayerData player = GetPlayer(playerData.NetworkId);
		if (player != null)
        {
			player.Position = playerData.Position;
        }
	}
	// Rpc
	public void PlayerNodeGenerated(PlayerData player)
    {
		foreach (var col in _chunks)
		{
			foreach (var row in col.Value)
			{
				ChunkData chunk = row.Value;
				int[] voxels = chunk.CreateFlatArray();
				_server.Network.RpcId(player.NetworkId, nameof(_server.Network.ChunkLoaded), chunk.X, chunk.Z, voxels);
			}
		}
	}
	// Rpc
	public void PlayerJoined(PlayerData player)
	{
		_players.Add(player);
	}
	// Rpc
	public void PlayerLeft(PlayerData player)
	{
		_players.Remove(player);
	}
	public WorldData(Server server)
	{
		_server = server;
		_chunkGenerator = new Thread(GeneratingLoop);
		_chunkGenerator.Start();
	}
	public List<PlayerData> Players
	{
		get { return _players; }
	}
}
