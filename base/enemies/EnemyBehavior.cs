using Godot;
using System;



public partial class EnemyBehavior : PathFollow2D
{
	[Export]
	public string EnemyName;

	private EnemyConfig config;

	private ProgressBar bar;
	private Control control;
	private AnimatedSprite2D walkingAnimation;
	private AudioStreamPlayer damageSound;
	private bool playerDamaged = false;

	public int HP
	{
		get; private set;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bar = GetNode<ProgressBar>("Control/ProgressBar");
		control = GetNode<Control>("Control");
		walkingAnimation = GetNode<AnimatedSprite2D>("CharacterBody2D/AnimatedSprite2D");
		damageSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		config = GlobalData.Instance.ENEMY_INFO[EnemyName];
		HP = config.StartHP;
		bar.MaxValue = HP;
		UpdateHPDisplay();

		walkingAnimation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Progress += config.Speed * (float)delta;

		if (ProgressRatio == 1)
		{
			Visible = false;
			if (!playerDamaged)
			{
				if (!damageSound.Playing)
				{
					damageSound.Play();
				}

				playerDamaged = true;
				GlobalData.Instance.TakeDamage(1);
			}
		}

		// bar should always be on top
		control.Rotation = -Rotation;
	}

	public void TakeDamage(int damage)
	{
		HP -= damage;
		if (HP <= 0)
		{
			// dead
			QueueFree();
		}
		else
		{
			UpdateHPDisplay();
		}
	}

	private void UpdateHPDisplay()
	{
		bar.Value = HP;
	}

	private void Destroy()
	{
		QueueFree();
	}
}
