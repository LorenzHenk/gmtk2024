using Godot;
using System;
using System.Collections.Generic;

public partial class BasicTurret : Node2D
{
	private TurretConfig config;
	private List<Simp> enemiesInRange;

	private Simp selectedEnemy;

	private Timer delayTimer;

	public bool built = false;

	private bool canFire = true;

	public Vector2 tileLocation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		config = GlobalData.Instance.TOWER_INFO["BasicTurret"];

		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").ApplyScale(new Vector2(config.Range, config.Range));
		GetNode<MeshInstance2D>("MeshInstance2D").ApplyScale(new Vector2(config.Range, config.Range));
		GetNode<MeshInstance2D>("MeshInstance2D").Modulate = new Color(0.7f, 0.7f, 0.7f, 0.1f);

		delayTimer = GetNode<Timer>("ShootingDelayTimer");

		delayTimer.WaitTime = config.ShotDelay;

		enemiesInRange = new List<Simp>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!built)
		{
			return;
		}

		if (enemiesInRange.Count > 0)
		{
			SelectEnemy();
			Turn();

			if (canFire)
			{
				Shoot();
			}

		}
		else
		{
			selectedEnemy = null;
		}
	}

	private void Shoot()
	{
		if (selectedEnemy != null && !selectedEnemy.IsQueuedForDeletion())
		{
			// TODO display bullet
			// TODO wait until bullet hit
			selectedEnemy.TakeDamage(config.Damage);


			canFire = false;
			delayTimer.Start();
		}
	}

	private void SelectEnemy()
	{
		Simp bestMatch = enemiesInRange[0];
		foreach (var enemy in enemiesInRange)
		{
			if (enemy.ProgressRatio > bestMatch.ProgressRatio)
			{
				bestMatch = enemy;
			}
		}

		selectedEnemy = bestMatch;
	}

	private void Turn()
	{
		if (selectedEnemy == null)
		{
			return;
		}

		LookAt(
selectedEnemy.Position
		);
	}

	public void BodyEnteredHandler(Node2D body)
	{
		enemiesInRange.Add(body.GetParent<Simp>());
	}

	public void BodyExitedHandler(Node2D body)
	{
		enemiesInRange.Remove(body.GetParent<Simp>());
	}

	public void DelayTimerDoneHandler()
	{
		canFire = true;
	}
}
