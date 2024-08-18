using Godot;
using System;

public partial class TopBar : HBoxContainer
{

	RichTextLabel moneyLabel;
	RichTextLabel hpLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		moneyLabel = GetNode<RichTextLabel>("Money");
		hpLabel = GetNode<RichTextLabel>("HP");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		moneyLabel.Text = "Resources: " + GlobalData.Instance.Resources.ToString();
		hpLabel.Text = "HP: " + GlobalData.Instance.HP.ToString();
	}

	public void PauseButtonHandler()
	{
		GetTree().Paused = !GetTree().Paused;
	}
}
