using Godot;

public partial class Global : Node
{
    public static Player ClientPlayer;
    public static string MultiplayerAddress;
    public static short MultiplayerPort;
    public static bool FailedToConnect = false;
    public static float MouseSensitivity = 0.2f;
    public static bool IsHost = false;
    public static bool IsInGame = false;
    public static Player Player;
    public static Global GlobalNode;
    public static NetworkNode Network;
    public Node Main;
    public void ChangeScene(PackedScene scene)
    {
        if (Main != null)
        {
            foreach (Node child in Main.GetChildren())
            {
                if (child.Name != "World")
                {
                    child.QueueFree();
                    Main.RemoveChild(child);
                }
            }
        }
        Main.AddChild(scene.Instantiate());
    }
    public void ChangeSceneToFile(string scene)
    {
        ChangeScene(ResourceLoader.Load<PackedScene>(scene));
    }
    public override void _Ready()
    {
        base._Ready();
        GlobalNode = this;
    }
}
