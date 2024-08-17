using Godot;

public partial class Asteroid : Node2D
{
	[Signal]
    public delegate void PlayerCollisionEventHandler();

	[Export]
	public double RotationSpeed = 0.25;

	private AnimatedSprite2D _animatedSprite;
	private Sprite2D _sprite;

    public override void _Ready()
    {
		PlayerCollision += OnPlayerCollision;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Rotate((float)(RotationSpeed * delta));
    }

    public void PlayExplodeAnimationHandler()
	{
		QueueFree();
	}

	private void OnPlayerCollision()
    {
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_sprite.Visible = false;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Visible = true;
        _animatedSprite.Play("explode");
    }
}
