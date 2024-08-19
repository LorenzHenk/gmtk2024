using Godot;
using System;

public partial class EndMenu : Control
{

	private Button quitGameButton;
	private Label wavesLabel;
	private Label resourcesLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		wavesLabel = GetNode<Label>("PanelContainer/MarginContainer/CenterContainer/VBoxContainer/GridContainer/Waves2Label");
		wavesLabel.Text = GlobalData.Instance.WaveCount.ToString();

		resourcesLabel = GetNode<Label>("PanelContainer/MarginContainer/CenterContainer/VBoxContainer/GridContainer/Resources2Label");
		
		if(GlobalData.Instance.ResourcesGathered > 1000)
		{
			resourcesLabel.Text = (GlobalData.Instance.ResourcesGathered / 1000).ToString("0.##") + "k";
		}
		else
		{
			resourcesLabel.Text = GlobalData.Instance.ResourcesGathered.ToString();
		}

		quitGameButton = GetNode<Button>("PanelContainer/MarginContainer/CenterContainer/VBoxContainer/QuitGameButton");
		if(OS.GetName().ToLower() == "ios")
		{
			quitGameButton.Visible = false;
		} 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnQuitGameButtonHandler()
	{
		if(OS.GetName().ToLower() != "ios")
		{
			GetTree().Quit();
		} 
	}
}
