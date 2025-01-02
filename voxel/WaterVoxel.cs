public partial class WaterVoxel : Voxel
{
	public static readonly int ID = 5;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = true;
	public static readonly WaterVoxel Instance = new();
	public WaterVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
