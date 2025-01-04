using Godot;

public class HighBeachLowerEdgeBiome : ForestBiome
{
    public static new readonly HighBeachLowerEdgeBiome Instance = new();
    public HighBeachLowerEdgeBiome()
    {
        BaseHeight = HighBeachEdgeBiome.Instance.BaseHeight - 6;
        HeightVariation = 8;
    }
}