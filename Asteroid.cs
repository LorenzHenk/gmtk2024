using Godot;

public partial class Asteroid : Node2D
{
	[Signal]
    public delegate void PlayerCollisionEventHandler();

    public override void _Ready()
    {
		PlayerCollision += OnPlayerCollision;
    }

	private void OnPlayerCollision()
    {
        this.QueueFree();
    }
}

