using Godot;

public partial class Player
{
	private Godot.Collections.Dictionary _dict = new();
	public Player(Godot.Collections.Dictionary dict)
	{
		_dict = dict;
	}
	public Player(ClientPlayer player)
	{
		NetworkId = player.NetworkId;
		Position = player.Position;
	}
	public Player()
	{
		
	}
	public long NetworkId
	{
		get 
		{
			return _dict["NetworkId"].AsInt64();
		}
		set 
		{
			_dict["NetworkId"] = value;
		}
	}
	public Vector3 Position
    {
		get
        {
			if (_dict.ContainsKey("Position"))
            {
				return _dict["Position"].AsVector3();
			}
			return Vector3.Zero;
        }
		set
        {
			_dict["Position"] = value;
        }
    }
	public Godot.Collections.Dictionary Dictionary
	{
		get
		{
			return _dict;
		}
	}
}
