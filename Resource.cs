using Godot;

public partial class Resource : Node2D
{
	[Signal]
    public delegate void PlayerCollisionEventHandler();
	private Sprite2D _sprite;

    public override void _Ready()
    {
		base._Ready();

		PlayerCollision += OnPlayerCollision;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
	}

    public void PlayExplodeAnimationHandler()
	{
		QueueFree();
	}

	private void OnPlayerCollision()
    {
		QueueFree();
    }
}
