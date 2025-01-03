public partial class ObsidianVoxel : Voxel
{
	public static readonly int ID = 7;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly ObsidianVoxel Instance = new();
	public ObsidianVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
