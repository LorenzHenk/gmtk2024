using Godot;
using System;

public partial class Space : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// attach scene-handler to liftoff button
		var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");

		var landButton = GetNode<Button>("Player/HUD/LandButton");
		// when running scene directly for debugging purposes
		if (sceneHandler != null)
		{
			landButton.Pressed += this.BackToBase;
		}
		else
		{
			landButton.QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void BackToBase()
	{
		var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");
		var player = GetNodeOrNull<Player>("Player");
		
		if(sceneHandler != null && player != null)
		{
			GlobalData.Instance.AddResources(player.CurrentResources);
			sceneHandler.BackToBase();
		}
	}
}
