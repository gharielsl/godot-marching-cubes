using Godot;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public partial class World : Node3D
{
	private Player _player;
	private Node3D _players;
	private Node3D _chunksNode;
	private Thread _chunkGenerator;
	private readonly PackedScene _chunkScene = ResourceLoader.Load<PackedScene>("res://world/chunk.tscn");
	private readonly Dictionary<int, Player> _playersDict = new();
	private readonly Dictionary<int, Dictionary<int, Chunk>> _chunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingChunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingPriorityChunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingBorderChunks = new();
	public override void _Ready()
	{
		base._Ready();
		_players = GetNode<Node3D>("Players");
		_chunksNode = GetNode<Node3D>("Chunks");
		_chunkGenerator = new Thread(GeneratingLoop);
		_chunkGenerator.Start();
	}
	private void HandleGeneratingTask(ConcurrentQueue<Chunk> queue)
	{
		if (queue != _generatingPriorityChunks && queue != _generatingBorderChunks)
		{
			HandleGeneratingTask(_generatingPriorityChunks);
			HandleGeneratingTask(_generatingBorderChunks);
		}
		while (!queue.IsEmpty)
		{
			if (queue != _generatingPriorityChunks && queue != _generatingBorderChunks)
			{
				HandleGeneratingTask(_generatingPriorityChunks);
				HandleGeneratingTask(_generatingBorderChunks);
			}
			queue.TryPeek(out Chunk chunk);
			if (chunk.IsDisposed)
			{
				queue.TryDequeue(out _);
			}
			else if (chunk.IsNodeReady() && !chunk.IsGenerating)
			{
				queue.TryDequeue(out chunk);
				chunk.Generate(queue == _generatingBorderChunks);
			}
		}
	}
	private void GeneratingLoop()
	{
		while (IsInsideTree())
		{
			HandleGeneratingTask(_generatingPriorityChunks);
			HandleGeneratingTask(_generatingChunks);
			HandleGeneratingTask(_generatingBorderChunks);
		}
	}
	public void Connected(PlayerData playerData, PlayerData[] existing)
	{
		PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://player/player.tscn");
		foreach (PlayerData existingPlayer in existing)
		{
			Player existingPlayerScene = playerScene.Instantiate<Player>();
			existingPlayerScene.NetworkId = existingPlayer.NetworkId;
			_playersDict[(int)existingPlayer.NetworkId] = existingPlayerScene;
			_players.AddChild(existingPlayerScene);
		}
		_player = playerScene.Instantiate<Player>();
		_player.NetworkId = playerData.NetworkId;
		_players.AddChild(_player);
	}
	public void PlayerJoined(PlayerData playerData)
	{
		if (playerData.NetworkId == Multiplayer.GetUniqueId())
		{
			return;
		}
		Player scenePlayer = ResourceLoader.Load<PackedScene>("res://player/player.tscn").Instantiate<Player>();
		scenePlayer.NetworkId = playerData.NetworkId;
		_playersDict[(int)playerData.NetworkId] = scenePlayer;
		_players.AddChild(scenePlayer);
	}
	public void PlayerLeft(PlayerData player)
	{
		for (int i = 0; i < _players.GetChildren().Count; i++)
		{
			if ((_players.GetChildren()[i] as Player).NetworkId == player.NetworkId)
			{
				_players.RemoveChild(_players.GetChildren()[i]);
			}
		}
	}
	public void PlayerUpdated(PlayerData playerData)
	{
		if (playerData.NetworkId == _player.NetworkId)
		{
			return;
		}
		_playersDict[(int)playerData.NetworkId].Position = playerData.Position;
	}
	public Chunk GetChunk(int x, int z)
	{
		if (!_chunks.ContainsKey(x))
		{
			return null;
		}
		if (_chunks[x].ContainsKey(z))
		{
			return _chunks[x][z];
		}
		return null;
	}
	public void ChunkLoaded(int x, int z, int[] voxels)
	{
		if (GetChunk(x, z) == null)
		{
			if (!_chunks.ContainsKey(x))
			{
				_chunks[x] = new Dictionary<int, Chunk>();
			}
			_chunks[x].Remove(z);
			Chunk newChunk = _chunkScene.Instantiate<Chunk>();
			_chunks[x].Add(z, newChunk);
		}
		Chunk chunk = _chunks[x][z];
		chunk.SetData(x, z, this, voxels);
		_generatingChunks.Enqueue(chunk);
		if (chunk.GetParent() == null)
		{
			chunk.Position = new Vector3(ChunkData.ChunkSize * x, 0, ChunkData.ChunkSize * z);
			_chunksNode.AddChild(chunk);
		}
	}
	public void ChunkUnloaded(int x, int z)
	{
		Chunk chunk = GetChunk(x, z);
		if (chunk != null)
		{
			chunk.QueueFree();
			if (_chunks.ContainsKey(x))
			{
				_chunks[x].Remove(z);
				if (_chunks[x].Count == 0)
				{
					_chunks.Remove(x);
				}
			}
		}
	}
	public Player Player
	{
		get { return _player; }
	}
}
