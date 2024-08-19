using Godot;
using System.Threading.Tasks;

public partial class Space : Node2D
{
	private AudioStreamPlayer theme;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		theme = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		theme.Play();

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

		if (!GlobalData.Instance.TutorialSeenSpace)
		{
			StartTutorial();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!theme.Playing)
		{
			theme.Play();
		}
	}

	public void BackToBase()
	{
		var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");
		var player = GetNodeOrNull<Player>("Player");

		if (sceneHandler != null && player != null)
		{
			GlobalData.Instance.AddResources(player.CurrentResources);
			sceneHandler.BackToBase();
		}
	}

	private async void StartTutorial()
	{
		GetNode<Panel>("Player/HUD/Tutorial").Visible = true;

		// to load everything before pausing
		await Task.Delay(500);
		GetTree().Paused = true;
	}

	private void FinishTutorial()
	{
		GetTree().Paused = false;
		GetNode<Panel>("Player/HUD/Tutorial").Visible = false;
		GlobalData.Instance.TutorialSeenSpace = true;

	}
}
