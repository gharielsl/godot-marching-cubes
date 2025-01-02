public partial class SandVoxel : Voxel
{
	public static readonly int ID = 4;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly SandVoxel Instance = new();
	public SandVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
