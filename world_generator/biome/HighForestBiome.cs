using Godot;
public class HighForestBiome : ForestBiome
{
    public static new readonly HighForestBiome Instance = new();
    public HighForestBiome()
    {
        HeightVariation = 3;
        BaseHeight += 10;
    }
}