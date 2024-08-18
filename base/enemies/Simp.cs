using Godot;
using System;



public partial class Simp : PathFollow2D
{


	[Export]
	private float speed = 250;

	private ProgressBar bar;
	private Control control;

	public int HP
	{
		get; private set;
	} = 2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bar = GetNode<ProgressBar>("Control/ProgressBar");
		control = GetNode<Control>("Control");
		bar.MaxValue = HP;
		UpdateHPDisplay();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Progress += speed * (float)delta;

		if (ProgressRatio == 1)
		{

			GlobalData.Instance.TakeDamage(1);
			QueueFree();
		}

		if (ProgressRatio > 0.25 && HP == 2)
		{
			TakeDamage(1);
		}

		// bar should always be on top
		control.Rotation = -Rotation;
	}

	private void TakeDamage(int damage)
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
