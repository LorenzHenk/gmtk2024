using Godot;
using System;

public partial class SceneHandler : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("MainMenu/PanelContainer/VB/NewGameButton").Connect(Button.SignalName.Pressed, Callable.From(NewGameHandler));
		GetNode("MainMenu/PanelContainer/VB/QuitButton").Connect(Button.SignalName.Pressed, Callable.From(QuitHandler));

		GlobalData.Instance.WaveStarted += BackToBase;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void NewGameHandler()
	{
		RemoveCurrentScene();

		var gameScene = GD.Load<PackedScene>("res://Space.tscn").Instantiate();
		AddChild(gameScene);
		GlobalData.Instance.StartGame();
	}


	public void QuitHandler()
	{
		GetTree().Quit();
	}

	private void RemoveCurrentScene()
	{

		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}

	public void LiftoffButtonHandler()
	{
		RemoveCurrentScene();

		var gameScene = GD.Load<PackedScene>("res://Space.tscn").Instantiate();
		AddChild(gameScene);
	}

	public void BackToBase()
	{
		if (GetChild(0) is Base)
		{
			return;
		}

		RemoveCurrentScene();

		var gameScene = GD.Load<PackedScene>("res://Base.tscn").Instantiate();
		AddChild(gameScene);
	}

	public void EndMenuHandler()
	{
		RemoveCurrentScene();

		var gameScene = GD.Load<PackedScene>("res://EndMenu.tscn").Instantiate();
		AddChild(gameScene);
	}
}
