using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public partial class Chunk : StaticBody3D
{
	public static readonly int BorderSize = 3;
	public static readonly int ChunkSizeWithBorder = ChunkData.ChunkSize + BorderSize * 2;

	private Voxel[,,] _data;
	private readonly ConcurrentQueue<Action> _frameQueue = new();
	private bool _generating = false, _disposed = false;
	private int _x, _z;
	private World _world;
	private CollisionShape3D _collision;
	private MeshInstance3D _mesh;
	public override void _Ready()
	{
		base._Ready();
		_collision = GetNode<CollisionShape3D>("Collision");
		_mesh = GetNode<MeshInstance3D>("Mesh");
		_collision.Shape = new ConcavePolygonShape3D();
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		while (!_frameQueue.IsEmpty)
		{
			_frameQueue.TryDequeue(out Action action);
			action();
		}
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		_disposed = true;
		if (_mesh != null)
		{
			_mesh.Dispose();
		}
		_collision.Dispose();
	}
	public void Generate(bool selfOnly)
	{
		_generating = true;
		List<Vector3> positions = new();
		List<int> indices = new();
		Dictionary<Vector3, int> uniquePositions = new();
		List<Vector3> tranPositions = new();
		List<int> tranIndices = new();
		Dictionary<Vector3, int> tranUniquePositions = new();
		for (int x = 0; x < ChunkSizeWithBorder; x++)
		{
			for (int y = 0; y < WorldData.WorldHeight; y++)
			{
				for (int z = 0; z < ChunkSizeWithBorder; z++)
				{
					MarchingCubes.MarchCube(
						new Vector3I(x, y, z),
						_data,
						positions,
						indices,
						uniquePositions,
						tranPositions,
						tranIndices,
						tranUniquePositions);
				}
			}
		}
		indices.Reverse();
		tranIndices.Reverse();
		Vector3[] collision = new Vector3[indices.Count];
		for (int i = 0; i < indices.Count; i++)
		{
			collision[i] = positions[indices[i]];
		}
		ArrayMesh arrayMesh = null;
		SurfaceTool surfaceTool = null;
		if (indices.Count > 0)
		{
			arrayMesh = new ArrayMesh();
			Godot.Collections.Array arrays = new Godot.Collections.Array();
			arrays.Resize((int)Mesh.ArrayType.Max);
			arrays[(int)Mesh.ArrayType.Vertex] = positions.ToArray();
			arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
			surfaceTool = new SurfaceTool();
			surfaceTool.CreateFromArrays(arrays);
			surfaceTool.GenerateNormals();
			arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceTool.CommitToArrays());
		}
		_frameQueue.Enqueue(() =>
		{
			if (_mesh.Mesh != null)
			{
				_mesh.Mesh.Dispose();
			}
			if (indices.Count > 0)
			{
				surfaceTool.Dispose();
				_mesh.Mesh = arrayMesh;
				((ConcavePolygonShape3D)_collision.Shape).SetFaces(collision);
			}
			_generating = false;
		});
	}
	public void SetData(int x, int z, World world, int[] data)
	{
		_x = x;
		_z = z;
		_world = world;
		Voxel[,,] voxels = _data;
		if (_data == null)
		{
			voxels = new Voxel[ChunkSizeWithBorder + 1, WorldData.WorldHeight + 1, ChunkSizeWithBorder + 1];
		}
		for (int xi = 0; xi < ChunkData.ChunkSize; xi++)
		{
			for (int yi = 0; yi < WorldData.WorldHeight; yi++)
			{
				for (int zi = 0; zi < ChunkData.ChunkSize; zi++)
				{
					int voxelId = data[xi + yi * ChunkData.ChunkSize + zi * ChunkData.ChunkSize * WorldData.WorldHeight];
					Voxel voxel = Voxel.Voxels[voxelId];
					voxels[BorderSize + xi, yi, BorderSize + zi] = voxel;
				}
			}
		}
		_data = voxels;
	}
	public int X
	{
		get { return _x; }
	}
	public int Z
	{
		get { return _z; }
	}
	public bool IsGenerating
	{
		get { return _generating; }
	}
	public bool IsDisposed
	{
		get { return _disposed; }
	}
}
