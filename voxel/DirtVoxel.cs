public partial class DirtVoxel : Voxel
{
	public static readonly int ID = 1;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly DirtVoxel Instance = new();
	public DirtVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{
		
	}
}
