using System;
using Godot;

public partial class Asteroid : Node2D
{
	[Signal]
    public delegate void PlayerCollisionEventHandler();

	private double RotationSpeed;

	private AnimatedSprite2D _animatedSprite;
	private Sprite2D _sprite;
	private AudioStreamPlayer2D destroyed;

    public override void _Ready()
    {
		base._Ready();

		destroyed = GetNode<AudioStreamPlayer2D>("../AudioStreamPlayer2D");
		RotationSpeed = GD.RandRange(-0.9, 0.9);
		Scale = Vector2.One * (float)GD.RandRange(0.75, 2.75);
		PlayerCollision += OnPlayerCollision;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		
		Rotate((float)(RotationSpeed * delta));
    }

    public void PlayExplodeAnimationHandler()
	{
		Visible = false;
		QueueFree();
	}

	public void OnDestroyAudioFinishedHandler()
	{
		GetParent().QueueFree();
	}

	private void OnPlayerCollision()
    {
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_sprite.Visible = false;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Visible = true;
        _animatedSprite.Play("explode");

		destroyed.Play();
    }
}
