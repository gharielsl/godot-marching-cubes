public partial class DirtVoxel : Voxel
{
	public static readonly int ID = 1;
	public static readonly float DENSITY = 0;
	public static readonly DirtVoxel Instance = new DirtVoxel();
	public DirtVoxel() : base(ID, DENSITY)
	{
		
	}
}
