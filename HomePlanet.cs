using System;
using Godot;

public partial class HomePlanet : Sprite2D
{
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		player = GetNode<Player>("../Player");
		Position = new Vector2(
			0, 
		    -(GlobalData.Instance.ChunkSize * GlobalData.Instance.SpaceGoal) - GlobalData.Instance.ChunkSize
		);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if(!Reached())
		{
			return;
		}

		Visible = true;
		Position = new Vector2(player.Position.X, Position.Y);
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		
		Rotate(0.05f * (float)delta);
    }

	private bool Reached()
	{
		var distance = GlobalData.Instance.SpaceGoal * GlobalData.Instance.ChunkSize;
		return distance <= Math.Abs(player.GlobalPosition.Y);
	}
}
