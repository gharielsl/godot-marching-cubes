using Godot;

public partial class Player : CharacterBody3D
{
	public float JumpHeight = 3.5f;
	public float Acceleration = 2;
	public float MaxSpeed = 10;
	public float Speed = 5;
	public float Deceleration = 2;
	public float Gravity = 10;

	private int _networkId;
	private float _jolt = 1;
	private Node3D _head;
	private Camera3D _camera;
	private CollisionShape3D _collision;
	private RayCast3D _spawnCheck;
	private Vector3 _velocity = new Vector3();
	private Vector2 _mouseInput = new Vector2();
	private Vector2 _input = new Vector2();
	public override void _EnterTree()
	{
		base._EnterTree();
		SetMultiplayerAuthority(_networkId);
	}
	public override void _Ready()
	{
		base._Ready();
		_head = GetNode<Node3D>("Head");
		_camera = _head.GetNode<Camera3D>("Camera");
		_collision = GetNode<CollisionShape3D>("Collision");
		_spawnCheck = GetNode<RayCast3D>("SpawnCheck");
		_camera.Current = _networkId == Multiplayer.GetUniqueId();
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
		{
			return;
		}
		_input = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
		Vector3 direction = new Vector3();
		direction += _input.X * _head.GlobalTransform.Basis.X;
		direction -= _input.Y * _head.GlobalTransform.Basis.Z;
		_velocity = _velocity.Lerp(direction * Speed, Acceleration * _jolt * (float)delta);

		if (IsOnFloor())
		{
			_jolt = 1;
			_velocity.Y = 0;
			if (Input.IsActionJustPressed("jump"))
			{
				_jolt = 0.1f;
				_velocity.Y = JumpHeight;
			}
		}
		else
		{
			_velocity.Y -= Gravity * (float)delta;
		}
		Vector3 rotation = _camera.Rotation;
		rotation.X -= _mouseInput.Y * Global.MouseSensitivity * (float)delta;
		rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
		_camera.Rotation = rotation;
		_head.RotateY(-_mouseInput.X * Global.MouseSensitivity * (float)delta);
		_mouseInput = Vector2.Zero;
		if (IsOnFloor())
		{
			Vector2 surfaceVelocity = new Vector2(_velocity.X, _velocity.Z);
			if (surfaceVelocity.Length() > MaxSpeed)
			{
				surfaceVelocity = surfaceVelocity.Normalized() * MaxSpeed;
			}
			if (_input.Length() == 0)
			{
				surfaceVelocity = surfaceVelocity.Lerp(Vector2.Zero, Deceleration*(float)delta);
			}
			_velocity.X = surfaceVelocity.X;
			_velocity.Z = surfaceVelocity.Y;
		}
		Velocity = _velocity;
		MoveAndSlide();
	}
	public void TeleportToTop()
	{
		_spawnCheck.Enabled = true;
		_spawnCheck.ForceRaycastUpdate();
		GlobalPosition = _spawnCheck.GetCollisionPoint() + new Vector3(0, 1.5f, 0);
		_spawnCheck.Enabled = false;
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
		{
			return;
		}
		if (Input.MouseMode != Input.MouseModeEnum.Captured)
		{
			return;
		}
		if (@event is InputEventMouseMotion mouseEvent)
		{
			_mouseInput = mouseEvent.Relative;
		}
	}
	public int NetworkId
	{
		get { return _networkId; }
		set { _networkId = value; }
	}
}
