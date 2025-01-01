using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public partial class Chunk : StaticBody3D
{
	public static readonly int BorderSize = 3;
	public static readonly int ChunkSizeWithBorder = ChunkData.ChunkSize + BorderSize * 2;

	public Action OnGenerated;
	private Voxel[,,] _data;
	private readonly ConcurrentQueue<Action> _frameQueue = new();
	private bool _generating = false, _disposed = false;
	private int _x, _z;
	private Chunk[] _neighbours = new Chunk[8];
	private bool[] _neighbourNeedsUpdate = new bool[8];
	private World _world;
	private CollisionShape3D _collision;
	private MeshInstance3D _mesh, _transparentMesh;
	private Image _texDataImage;
	private ImageTexture _texData;
	private byte[] _texDataBuffer;
	public override void _Ready()
	{
		base._Ready();
		_texDataImage = Image.CreateEmpty(ChunkSizeWithBorder * ChunkSizeWithBorder, WorldData.WorldHeight, false, Image.Format.R8);
		_texData = ImageTexture.CreateFromImage(_texDataImage);
		_collision = GetNode<CollisionShape3D>("Collision");
		_mesh = GetNode<MeshInstance3D>("Mesh");
		_transparentMesh = GetNode<MeshInstance3D>("TransparentMesh");
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
		if (_transparentMesh != null)
		{
			_transparentMesh.Dispose();
		}
		if (_texData != null)
		{
			_texDataImage.Dispose();
			_texData.Dispose();
		}
		_collision.Dispose();
	}
	private void UpdateNeighbours()
	{
		_neighbours[0] = _world.GetChunk(_x + 1, _z);
		_neighbours[1] = _world.GetChunk(_x - 1, _z);
		_neighbours[2] = _world.GetChunk(_x, _z + 1);
		_neighbours[3] = _world.GetChunk(_x, _z - 1);
		_neighbours[4] = _world.GetChunk(_x + 1, _z + 1);
		_neighbours[5] = _world.GetChunk(_x + 1, _z - 1);
		_neighbours[6] = _world.GetChunk(_x - 1, _z + 1);
		_neighbours[7] = _world.GetChunk(_x - 1, _z - 1);
		int B = BorderSize;
		int S = ChunkData.ChunkSize;
		int SB = ChunkSizeWithBorder;
		int H = WorldData.WorldHeight;
		for (int a = 0; a < SB - B; a++)
		{
			for (int y = 0; y < H; y++)
			{
				for (int i = 0; i < B; i++)
				{
					int oa = B - a - 1;
					int ri = B - i - 1;
					if (a < BorderSize)
					{
						if (_neighbours[4] != null)
						{
							try
							{
								if (_neighbours[4]._data[oa, y, ri] != GetVoxel(ChunkData.ChunkSize - a - 1, y, ChunkData.ChunkSize - i - 1))
								{
									_neighbourNeedsUpdate[4] = true;
									_neighbours[4]._data[oa, y, ri] = GetVoxel(ChunkData.ChunkSize - a - 1, y, ChunkData.ChunkSize - i - 1);
								}
								_data[ChunkSizeWithBorder - oa - 1, y, ChunkSizeWithBorder - ri - 1] = _neighbours[4].GetVoxel(a, y, i);
							}catch{}
						}
						if (_neighbours[5] != null)
						{
							try
							{
								if (_neighbours[5]._data[oa, y, ChunkSizeWithBorder - ri - 1] != GetVoxel(ChunkData.ChunkSize - a - 1, y, i))
								{
									_neighbourNeedsUpdate[5] = true;
									_neighbours[5]._data[oa, y, ChunkSizeWithBorder - ri - 1] = GetVoxel(ChunkData.ChunkSize - a - 1, y, i);
								}
								_data[ChunkSizeWithBorder - oa - 1, y, ri] = _neighbours[5].GetVoxel(a, y, ChunkData.ChunkSize - i - 1);
							}catch { }

						}
						if (_neighbours[6] != null)
						{
							try
							{
								if (_neighbours[6]._data[ChunkSizeWithBorder - oa - 1, y, ri] != GetVoxel(a, y, ChunkData.ChunkSize - i - 1))
								{
									_neighbourNeedsUpdate[6] = true;
									_neighbours[6]._data[ChunkSizeWithBorder - oa - 1, y, ri] = GetVoxel(a, y, ChunkData.ChunkSize - i - 1);
								}
								_data[oa, y, ChunkSizeWithBorder - ri - 1] = _neighbours[6].GetVoxel(ChunkData.ChunkSize - a - 1, y, i);
							}catch { }
						}
						if (_neighbours[7] != null)
						{
							try
							{
								if (_neighbours[7]._data[ChunkSizeWithBorder - oa - 1, y, ChunkSizeWithBorder - ri - 1] != GetVoxel(a, y, i))
								{
									_neighbourNeedsUpdate[7] = true;
									_neighbours[7]._data[ChunkSizeWithBorder - oa - 1, y, ChunkSizeWithBorder - ri - 1] = GetVoxel(a, y, i);
								}
								_data[oa, y, ri] = _neighbours[7].GetVoxel(ChunkData.ChunkSize - a - 1, y, ChunkData.ChunkSize - i - 1);
							}catch { }
						}
						continue;
					}
					ri = BorderSize - i;
					oa = a - BorderSize;
					if (_neighbours[0] != null)
					{
						try
						{
							if (_neighbours[0]._data != null)
							{
								if (_neighbours[0]._data[ri - 1, y, oa + BorderSize] != GetVoxel(ChunkData.ChunkSize - i - 1, y, oa))
								{
									_neighbourNeedsUpdate[0] = true;
									_neighbours[0]._data[ri - 1, y, oa + BorderSize] = GetVoxel(ChunkData.ChunkSize - i - 1, y, oa);
								}
								_data[ChunkSizeWithBorder - ri, y, oa + BorderSize] = _neighbours[0].GetVoxel(i, y, oa);
							}
						}catch { }
					}
					if (_neighbours[1] != null)
					{
						try
						{
							if (_neighbours[1]._data != null)
							{
								if (_neighbours[1]._data[ChunkSizeWithBorder - ri, y, oa + BorderSize] != GetVoxel(i, y, oa))
								{
									_neighbourNeedsUpdate[1] = true;
									_neighbours[1]._data[ChunkSizeWithBorder - ri, y, oa + BorderSize] = GetVoxel(i, y, oa);
								}
								_data[ri - 1, y, oa + BorderSize] = _neighbours[1].GetVoxel(ChunkData.ChunkSize - i - 1, y, oa);
							}
						}catch { }
					}
					if (_neighbours[2] != null)
					{
						try
						{
							if (_neighbours[2]._data != null)
							{
								if (_neighbours[2]._data[oa + BorderSize, y, ri - 1] != GetVoxel(oa, y, ChunkData.ChunkSize - i - 1))
								{
									_neighbourNeedsUpdate[2] = true;
									_neighbours[2]._data[oa + BorderSize, y, ri - 1] = GetVoxel(oa, y, ChunkData.ChunkSize - i - 1);
								}
								_data[oa + BorderSize, y, ChunkSizeWithBorder - ri] = _neighbours[2].GetVoxel(oa, y, i);
							}
						}catch { }
					}
					if (_neighbours[3] != null)
					{
						try
						{
							if (_neighbours[3]._data != null)
							{
								if (_neighbours[3]._data[oa + BorderSize, y, ChunkSizeWithBorder - ri] != GetVoxel(oa, y, i))
								{
									_neighbourNeedsUpdate[3] = true;
									_neighbours[3]._data[oa + BorderSize, y, ChunkSizeWithBorder - ri] = GetVoxel(oa, y, i);
								}
								_data[oa + BorderSize, y, ri - 1] = _neighbours[3].GetVoxel(oa, y, ChunkData.ChunkSize - i - 1);
							}
						}catch { }
					}
				}
			}	
		}
	}
	private void CreateMesh(List<Vector3> positions, List<int> indices, MeshInstance3D mesh)
	{
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
			if (mesh.Mesh != null)
			{
				mesh.Mesh.Dispose();
				mesh.Mesh = null;
			}
			if (indices.Count > 0)
			{
				surfaceTool.Dispose();
				mesh.Mesh = arrayMesh;
			}
		});
	}
	public void Generate(bool selfOnly)
	{
		_generating = true;
		if (!selfOnly)
		{
			UpdateNeighbours();
		}
		List<Vector3> positions = new();
		List<int> indices = new();
		Dictionary<Vector3, int> uniquePositions = new();
		List<Vector3> tranPositions = new();
		List<int> tranIndices = new();
		Dictionary<Vector3, int> tranUniquePositions = new();
		_texDataBuffer = new byte[ChunkSizeWithBorder * ChunkSizeWithBorder * WorldData.WorldHeight];
		for (int x = 0; x < ChunkSizeWithBorder; x++)
		{
			for (int y = 0; y < WorldData.WorldHeight; y++)
			{
				for (int z = 0; z < ChunkSizeWithBorder; z++)
				{
					Voxel voxel = _data[x, y, z];
					int voxelId = AirVoxel.ID;
					if (voxel != null)
					{
						voxelId = voxel.Id;
					}
					int index = (y * ChunkSizeWithBorder * ChunkSizeWithBorder + (z * ChunkSizeWithBorder + x));
					byte[] voxelIdBytes = BitConverter.GetBytes(voxelId);
					_texDataBuffer[index] = (byte)voxelId;
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
		if (!selfOnly)
		{
			for (int i = 0; i < _neighbourNeedsUpdate.Length; i++)
			{
				if (_neighbourNeedsUpdate[i])
				{
					_world.GeneratingBorderChunks.Enqueue(_neighbours[i]);
				}
			}
		}
		GeometrySmoothing.SmoothGeometry(positions, indices);
		GeometrySmoothing.SmoothGeometry(tranPositions, tranIndices);
		GeometrySmoothing.SubdivideGeometry(positions, indices);
		GeometrySmoothing.SmoothGeometry(positions, indices);
		List<Vector3> collision = new();
		for (int i = 0; i < indices.Count; i += 3)
		{
			Vector3 v0 = positions[indices[i]];
			Vector3 v1 = positions[indices[i + 1]];
			Vector3 v2 = positions[indices[i + 2]];
			Vector3 center = (v0 + v1 + v2) / 3;
			if (center.X > BorderSize && center.Z > BorderSize &&
				center.X <= ChunkData.ChunkSize + BorderSize &&
				center.Z <= ChunkData.ChunkSize + BorderSize)
			{
				collision.Add(v0);
				collision.Add(v1);
				collision.Add(v2);
			}
		}
		CreateMesh(positions, indices, _mesh);
		CreateMesh(tranPositions, tranIndices, _transparentMesh);
		_frameQueue.Enqueue(() =>
		{
			_collision.Shape.Dispose();
			if (indices.Count > 0)
			{
				((ConcavePolygonShape3D)_collision.Shape).SetFaces(collision.ToArray());
			}
			if (_texDataBuffer != null)
			{
				_texDataImage.SetData(_texDataImage.GetWidth(), _texDataImage.GetHeight(), false, _texDataImage.GetFormat(), _texDataBuffer);
				_texData.Dispose();
				_texData = ImageTexture.CreateFromImage(_texDataImage);
				((ShaderMaterial)_mesh.GetActiveMaterial(0)).SetShaderParameter("chunkData", _texData);
				_texDataBuffer = null;
			}
			if (OnGenerated != null)
			{
				OnGenerated();
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
	public Voxel GetVoxel(int x, int y, int z)
	{
		if (x < 0 || y < 0 || z < 0) return AirVoxel.Instance;
		if ((x > ChunkData.ChunkSize - 1) || (y > WorldData.WorldHeight - 1) || (z > ChunkData.ChunkSize - 1))
		{
			return AirVoxel.Instance;
		}
		return _data[x + BorderSize, y, z + BorderSize];
	}
	public void SetVoxel(int x, int y, int z, Voxel voxel)
	{
		if (x < 0 || y < 0 || z < 0 || x >= ChunkData.ChunkSize || y >= WorldData.WorldHeight || z >= ChunkData.ChunkSize)
		{
			return;
		}
		_data[x + BorderSize, y, z + BorderSize] = voxel;
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
