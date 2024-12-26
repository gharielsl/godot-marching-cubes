using Godot;

public partial class Global : Node
{
    public static ClientPlayer ClientPlayer;
    public static string MultiplayerAddress;
    public static short MultiplayerPort;
    public static bool FailedToConnect = false;
    public static float MouseSensitivity = 0.2f;
    public static bool IsHost = false;
    public static Global GlobalNode;

    public Node Main;
    
    public void ChangeScene(PackedScene scene)
    {
        if (Main != null)
        {
            Main.GetChild(0).QueueFree();
            Main.RemoveChild(Main.GetChild(0));
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
