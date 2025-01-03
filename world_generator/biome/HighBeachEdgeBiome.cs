public class HighBeachEdgeBiome : ForestBiome
{
    public static new readonly HighBeachEdgeBiome Instance = new();
    public HighBeachEdgeBiome()
    {
        BaseHeight = HighBeachBiome.Instance.BaseHeight + 1;
        HeightVariation = 1;
    }
}