public partial class MossStoneVoxel : Voxel
{
	public static readonly int ID = 6;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly MossStoneVoxel Instance = new();
	public MossStoneVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
