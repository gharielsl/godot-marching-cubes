using Godot;

public partial class Player
{
	private Godot.Collections.Dictionary _dict;
	public Player(Godot.Collections.Dictionary dict)
	{
		_dict = dict;
	}
	public Player()
	{
		_dict = new Godot.Collections.Dictionary();
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
	public Godot.Collections.Dictionary Dictionary
	{
		get
		{
			return _dict;
		}
	}
}
