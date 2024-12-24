using System.Collections.Generic;
using System.Threading;

public partial class WorldData
{
	public static readonly int WorldHeight = 128;
	public static readonly int ChunkRenderDistance = 16;

	private readonly Dictionary<int, Dictionary<int, ChunkData>> _chunks = new Dictionary<int, Dictionary<int, ChunkData>>();
	private Thread _generatingThread;
	private void GeneratingLoop()
	{
		
	}
	public WorldData()
	{
		_generatingThread = new Thread(GeneratingLoop);
	}
}
