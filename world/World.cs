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
	private WorldEnvironment _environment;
	private DirectionalLight3D _sun;
	private DirectionalLight3D _moon;
	private bool _playerSpawned = false;
	private bool _playerTeleported = false;
	private bool _isInsideTree = false;
	private readonly PackedScene _chunkScene = ResourceLoader.Load<PackedScene>("res://world/chunk.tscn");
	private readonly Dictionary<int, Player> _playersDict = new();
	private readonly Dictionary<int, Dictionary<int, Chunk>> _chunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingChunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingPriorityChunks = new();
	private readonly ConcurrentQueue<Chunk> _generatingBorderChunks = new();
	public override void _Ready()
	{
		base._Ready();
		_isInsideTree = true;
		_players = GetNode<Node3D>("Players");
		_chunksNode = GetNode<Node3D>("Chunks");
		_environment = GetNode<WorldEnvironment>("Environment");
		_sun = GetNode<DirectionalLight3D>("Sun");
		_moon = GetNode<DirectionalLight3D>("Moon");
		_chunkGenerator = new Thread(GeneratingLoop);
		_chunkGenerator.Start();
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		_isInsideTree = false;
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
		while (_isInsideTree)
		{
			HandleGeneratingTask(_generatingPriorityChunks);
			HandleGeneratingTask(_generatingChunks);
			HandleGeneratingTask(_generatingBorderChunks);
		}
	}
	public void RegenerateChunks()
	{
		foreach (int x in _chunks.Keys)
		{
			foreach (int y in _chunks[x].Keys)
			{
				foreach (var z in _chunks[x])
				{
					_generatingChunks.Enqueue(z.Value);
				}
			}
		}
	}
	public void Connected(PlayerData playerData, PlayerData[] existing)
	{
		PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://player/player.tscn");
		foreach (PlayerData existingPlayer in existing)
		{
			Player existingPlayerScene = playerScene.Instantiate<Player>();
			existingPlayerScene.NetworkId = existingPlayer.NetworkId;
			_playersDict[existingPlayer.NetworkId] = existingPlayerScene;
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
		_playersDict[playerData.NetworkId] = scenePlayer;
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
		_playersDict[playerData.NetworkId].Position = playerData.Position;
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
			WorldDataUtils.WorldToChunk((int)_player.Position.X, (int)_player.Position.Z, out int playerChunkX, out int playerChunkZ, out int _, out int _);
			if (!_playerSpawned && playerChunkX == x && playerChunkZ == z)
			{
				_playerSpawned = true;
				chunk.OnGenerated = () =>
				{
					if (!_playerTeleported)
					{
						_playerTeleported = true;
						_player.TeleportToTop();
					}
				};
			}
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
	public void WorldTimeUpdated(double worldTime)
	{
		double timeInDay = worldTime % WorldData.DayDuration;
		double angleInDegrees = -(timeInDay / WorldData.DayDuration) * 360.0;
		_sun.Rotation = new Vector3(Mathf.DegToRad((float)angleInDegrees), 0, 0);
		_moon.Rotation = new Vector3(-Mathf.DegToRad(180 - (float)angleInDegrees), 0, 0);
		if (worldTime < WorldData.DayDuration / 2)
		{
			_sun.SkyMode = DirectionalLight3D.SkyModeEnum.LightAndSky;
			_environment.Environment.VolumetricFogAlbedo = Colors.Gray;
			_environment.Environment.VolumetricFogEmission = new Color(0.424f, 0.424f, 0.424f);
		}
		else
		{
			_sun.SkyMode = DirectionalLight3D.SkyModeEnum.SkyOnly;
			_environment.Environment.VolumetricFogAlbedo = Color.FromHtml("#04001b");
			_environment.Environment.VolumetricFogEmission = Color.FromHtml("#04001b");
		}
	}
	public void PlaceVoxels(Vector3I[] positions, int[] voxels)
	{
		HashSet<Chunk> toUpdate = new();
		for (int i = 0; i < positions.Length; i++)
		{
			Vector3I position = positions[i];
			WorldDataUtils.WorldToChunk(position.X, position.Z, out int chunkX, out int chunkZ, out int inChunkX, out int inChunkZ);
			Chunk chunk = GetChunk(chunkX, chunkZ);
			if (chunk != null && Voxel.Voxels.ContainsKey(voxels[i]))
			{
				chunk.SetVoxel(inChunkX, position.Y, inChunkZ, Voxel.Voxels[voxels[i]]);
				toUpdate.Add(chunk);
			}
		}
		foreach (Chunk chunk in toUpdate)
		{
			_generatingPriorityChunks.Enqueue(chunk);
		}
	}
	public Player Player
	{
		get { return _player; }
	}
	public ConcurrentQueue<Chunk> GeneratingChunks
	{
		get { return _generatingChunks; }
	}
	public ConcurrentQueue<Chunk> GeneratingPriorityChunks
	{
		get { return _generatingPriorityChunks; }
	}
	public ConcurrentQueue<Chunk> GeneratingBorderChunks
	{
		get { return _generatingBorderChunks; }
	}
}
