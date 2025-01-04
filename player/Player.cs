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
	private bool _shouldTeleportToTop = false;
	private Node3D _head;
	private Camera3D _camera;
	private CollisionShape3D _collision;
	private RayCast3D _spawnCheck;
	private PauseMenu _pauseMenu;
	private Vector3 _velocity = new Vector3();
	private Vector2 _mouseInput = new Vector2();
	private Vector2 _input = new Vector2();
	public Input.MouseModeEnum MouseModeBeforePause;
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
		_pauseMenu = GetNode<PauseMenu>("PauseMenu");
		_camera.Current = _networkId == Multiplayer.GetUniqueId();
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
		{
			return;
		}
		Global.Player = this;

		if (_shouldTeleportToTop)
		{
			_spawnCheck.Enabled = true;
			_spawnCheck.ForceRaycastUpdate();
			if (_spawnCheck.IsColliding())
			{
				GlobalPosition = _spawnCheck.GetCollisionPoint() + new Vector3(0, 1.5f, 0);
				_spawnCheck.Enabled = false;
				_shouldTeleportToTop = false;
			}
		}

		_input = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
		Vector3 direction = new Vector3();
		direction += _input.X * _head.GlobalTransform.Basis.X;
		direction -= _input.Y * _head.GlobalTransform.Basis.Z;
		Vector2 surfaceVelocity = new Vector2(_velocity.X, _velocity.Z);
		surfaceVelocity = surfaceVelocity.Lerp(new Vector2(direction.X, direction.Z) * Speed, Acceleration * _jolt * (float)delta);
		_velocity.X = surfaceVelocity.X;
		_velocity.Z = surfaceVelocity.Y;

		if (IsOnFloor())
		{
			_jolt = 1;
			_velocity.Y = 0;
			//if (Input.IsActionJustPressed("jump"))
			//{
			//	_jolt = 0.1f;
			//	_velocity.Y = JumpHeight;
			//}
		}
		else
		{
			_velocity.Y -= Gravity * (float)delta;
		}
		if (Input.IsActionJustPressed("jump"))
		{
			_jolt = 0.1f;
			Vector3 normal = IsOnFloor() ? GetFloorNormal() : Vector3.Up;
			_velocity += normal * JumpHeight;
			_velocity.Y = JumpHeight;
		}
		Vector3 rotation = _camera.Rotation;
		rotation.X -= _mouseInput.Y * Global.MouseSensitivity * (float)delta;
		rotation.X = Mathf.Clamp(rotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
		_camera.Rotation = rotation;
		_head.RotateY(-_mouseInput.X * Global.MouseSensitivity * (float)delta);
		_mouseInput = Vector2.Zero;
		if (IsOnFloor())
		{
			surfaceVelocity = new Vector2(_velocity.X, _velocity.Z);
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
		if (IsOnFloor())
		{
			Velocity *= (Velocity.Dot(GetFloorNormal()) + 4) / 4;
		}
		MoveAndSlide();
	}
	private void PlaceVoxel(Voxel voxel)
	{
		Vector2 center = GetWindow().GetVisibleRect().GetCenter();
		Vector3 origin = _camera.ProjectRayOrigin(center);
		Vector3 end = origin + _camera.ProjectRayNormal(center) * 100;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
		Godot.Collections.Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(query);
		if (result == null || result.Count == 0)
		{
			return;
		}
		Vector3 position = (Vector3)result["position"];
		Vector3 normal = (Vector3)result["normal"];
		Node3D collider = (Node3D)result["collider"];
		if (collider is Chunk chunk)
		{
			position -= chunk.Position;
			Vector3I positionI = new((int)position.X, (int)position.Y, (int)position.Z);
			Godot.Collections.Array<Vector3I> positions = new();
			positions.Resize(8);
			int[] voxels = new int[8];
			int i = 0;
			for (int x = 0; x <= 1; x++)
			{
				for (int y = 0; y <= 1; y++)
				{
					for (int z = 0; z <= 1; z++)
					{
						Vector3I currentPosition = positionI + new Vector3I(x - Chunk.BorderSize, y, z - Chunk.BorderSize);
						positions[i] = WorldDataUtils.ChunkToWorld(currentPosition.X, currentPosition.Y, currentPosition.Z, chunk.X, chunk.Z);
						voxels[i] = voxel.Id;
						i++;
					}
				}
			}
			Global.Network.Rpc(nameof(Global.Network.PlaceVoxels), positions, voxels);
		}
	}
	public void TeleportToTop()
	{
		_shouldTeleportToTop = true;
	}
	public void UnPause()
	{
		Input.MouseMode = MouseModeBeforePause;
		_pauseMenu.Visible = false;
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
		{
			return;
		}
		if (Input.MouseMode == Input.MouseModeEnum.Captured && @event is InputEventMouseMotion mouseEvent)
		{
			_mouseInput = mouseEvent.Relative;
		}
		if (Input.IsActionJustPressed("break"))
		{
			PlaceVoxel(AirVoxel.Instance);
		}
		if (Input.IsActionJustPressed("place"))
		{
			PlaceVoxel(DirtVoxel.Instance);
		}
	}
	public int NetworkId
	{
		get { return _networkId; }
		set { _networkId = value; }
	}
}
