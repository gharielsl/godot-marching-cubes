public partial class StoneVoxel : Voxel
{
	public static readonly int ID = 3;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly StoneVoxel Instance = new();
	public StoneVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
