using Godot;
using System;

public partial class BasicTurret : Node2D
{
	[Export]
	public int Damage
	{
		get; private set;
	}

	[Export]
	public int Range
	{
		get; private set;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").ApplyScale(new Vector2(Range, Range));
		GetNode<MeshInstance2D>("MeshInstance2D").ApplyScale(new Vector2(Range, Range));
		GetNode<MeshInstance2D>("MeshInstance2D").Modulate = new Color(0.7f, 0.7f, 0.7f, 0.1f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var position = GetGlobalMousePosition();

		LookAt(position);
	}
}
