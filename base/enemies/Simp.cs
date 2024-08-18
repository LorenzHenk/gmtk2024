using Godot;
using System;



public partial class Simp : PathFollow2D
{
	private EnemyConfig config;

	private ProgressBar bar;
	private Control control;

	public int HP
	{
		get; private set;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bar = GetNode<ProgressBar>("Control/ProgressBar");
		control = GetNode<Control>("Control");

		config = GlobalData.Instance.ENEMY_INFO["Simp"];
		HP = config.StartHP;
		bar.MaxValue = HP;
		UpdateHPDisplay();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Progress += config.Speed * (float)delta;

		if (ProgressRatio == 1)
		{

			GlobalData.Instance.TakeDamage(1);
			QueueFree();
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
}
