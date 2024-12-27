public partial class AirVoxel : Voxel
{
    public static readonly int ID = 0;
    public static readonly float DENSITY = 1;
    public static readonly bool IS_TRANSPARENT = false;
    public static readonly AirVoxel Instance = new();
    public AirVoxel() : base(ID, DENSITY, IS_TRANSPARENT)
    {
        
    }
}
