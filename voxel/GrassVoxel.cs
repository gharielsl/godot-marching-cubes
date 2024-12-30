public partial class GrassVoxel : Voxel
{
	public static readonly int ID = 2;
	public static readonly float DENSITY = 0;
	public static readonly bool IS_TRANSPARENT = false;
	public static readonly GrassVoxel Instance = new();
	public GrassVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
	{

	}
}
