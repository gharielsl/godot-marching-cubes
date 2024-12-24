public partial class AirVoxel : Voxel
{
    public static readonly int ID = 0;
    public static readonly float DENSITY = 1;
    public static readonly AirVoxel Instance = new AirVoxel();
    public AirVoxel() : base(ID, DENSITY)
    {
        
    }
}
