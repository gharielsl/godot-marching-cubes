public class HighBeachBiome : BeachBiome
{
    public static new readonly HighBeachBiome Instance = new();
    public HighBeachBiome()
    {
        BaseHeight = ForestBiome.Instance.BaseHeight + 5;
        HeightVariation = 1;
    }
}