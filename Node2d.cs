using Godot;
using System;
using System.Diagnostics;

public partial class Node2d : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print("Hello");
		var child = GetChild<Sprite2D>(0);
		Color c = (child.Texture as GradientTexture2D).Gradient.GetColor(0);


		(child.Texture as GradientTexture2D).Gradient.SetColor(0, c.Lerp(new Color(1, 0, 0), 0.01f));

	}
}
