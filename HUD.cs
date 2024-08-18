using Godot;
using System;
using System.Linq;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void PlayerDamageEventHandler();

	private void InitializePlayerHealth()
	{
		var player = GetParent<Player>();
		var hearts = GetNode<Node2D>("HB/Hearts");
		var heartTexture = (Texture2D)GD.Load("res://resources/heart.png");
		for (int i = player.Health; i > 0; i--)
		{
			Sprite2D heart = new Sprite2D
			{
				Texture = heartTexture,
				Position = new Vector2(i * 32, 32)
			};
			hearts.AddChild(heart);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		PlayerDamage += OnPlayerDamage;
		InitializePlayerHealth();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);


		var timeUntilNextWaveLabel = GetNode<Label>("HB/TimeUntilNextWaveLabel");
		timeUntilNextWaveLabel.Text = GlobalData.Instance.GetSecondsLeftUntilNextWave() + " seconds until next wave";
	}

	private void OnPlayerDamage()
	{
		if (!GetParent<Player>().Damaged)
		{
			var hearts = GetNode<Node2D>("HB/Hearts");
			var heart = hearts
				.GetChildren()
				.OfType<Sprite2D>()
				.FirstOrDefault(c => c.Visible);

			if (heart != null)
			{
				heart.Visible = false;
			}
		}
	}


}
