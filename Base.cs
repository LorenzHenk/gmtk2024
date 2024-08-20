using Godot;
using System.Threading.Tasks;

public partial class Base : Node2D
{

	private AudioStreamPlayer backgroundMusic;

	private bool IsInBuildMode { get; set; }

	private bool CanPlaceTower { get; set; }


	private TileMapLayer tileMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backgroundMusic = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		backgroundMusic.Play();

		tileMap = GetNode<TileMapLayer>("TileMapLayer");

		var turretBuy = GetNode<BuyTower>("UI/HUD/BuildTowers/BasicTurret");
		turretBuy.baseInstance = this;

		// load data from GlobalData.Instance.TowerData
		InitializeTowers();


		// attach scene-handler to liftoff button
		var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");

		var liftoffButton = GetNode<Button>("UI/HUD/LiftoffButton");
		// when running scene directly for debugging purposes
		if (sceneHandler != null)
		{
			liftoffButton.Pressed += sceneHandler.LiftoffButtonHandler;
		}
		else
		{
			liftoffButton.QueueFree();
		}


		if (GlobalData.Instance.WaveIsActive)
		{
			StartWave();
		}

		GlobalData.Instance.WaveStarted += StartWave;

		if (!GlobalData.Instance.TutorialSeenBase)
		{
			StartTutorial();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!backgroundMusic.Playing)
		{
			backgroundMusic.Play();
		}

		if (IsInBuildMode)
		{
			UpdateTowerPlacementPreview();
		}

		var hasChildren = GetNode<Path2D>("Path2D").GetChildCount() > 0;

		var liftoffButton = GetNodeOrNull<Button>("UI/HUD/LiftoffButton");
		if (liftoffButton != null)
		{
			liftoffButton.Visible = !hasChildren && !GlobalData.Instance.WaveIsActive;
		}

		if (!hasChildren && GlobalData.Instance.WaveIsActive)
		{
			GlobalData.Instance.EndWave();
		}

		var timeUntilNextWaveLabel = GetNodeOrNull<Label>("UI/HUD/TopBar/TimeUntilNextWaveLabel");
		timeUntilNextWaveLabel.Text = GlobalData.Instance.GetSecondsLeftUntilNextWave() + " seconds until next wave";

		var currentWaveLabel = GetNodeOrNull<Label>("UI/HUD/TopBar/CurrentWaveLabel");
		currentWaveLabel.Text = "Current wave: " + GlobalData.Instance.CurrentWave;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);


		if (@event is InputEventMouse)
		{
			var evt = (InputEventMouse)@event;

			var globalClicked = GetGlobalMousePosition();

			var currentTile =
				tileMap.LocalToMap(ToLocal(globalClicked));

			var tilePosition = ToGlobal(tileMap.MapToLocal(currentTile));

			if (evt.ButtonMask == MouseButtonMask.Right && @event.IsPressed())
			{
				if (GlobalData.Instance.TowerData.ContainsKey(currentTile))
				{
					GlobalData.Instance.RemoveTower(currentTile);
					// remove tower instance
					var children = GetNode("Towers").GetChildren();
					foreach (var child in children)
					{
						if (((BasicTurret)child).tileLocation == currentTile)
						{
							child.QueueFree();
							break;
						}
					}
				}
			}
			else if (evt.ButtonMask == MouseButtonMask.Left && @event.IsPressed())
			{
				// build tower

				if (!IsInBuildMode)
				{
					return;
				}
				if (CanPlaceTower)
				{
					GlobalData.Instance.AddTower("BasicTurret", currentTile);

					var tower = GetNode<BasicTurret>("UI/TowerPreview/DraggingTower");
					GetNode("UI/TowerPreview").RemoveChild(tower);

					GetNode("Towers").AddChild(tower);

					tower.Modulate = new Color(1, 1, 1, 1);
					tower.Position = tilePosition;
					tower.built = true;
					tower.tileLocation = currentTile;

					CancelBuildMode();
				}

			}
		}
	}


	public void EnablePlacementMode(int price, PackedScene towerScene, Vector2 mousePosition)
	{
		if (IsInBuildMode)
		{
			// already placing a tower, can't buy another one right now
			return;
		}

		IsInBuildMode = true;

		GlobalData.Instance.Buy(price);

		var draggingTower = towerScene.Instantiate<Node2D>();

		draggingTower.Name = "DraggingTower";


		var control = new Control();

		control.AddChild(draggingTower, true);

		control.Name = "TowerPreview";

		GetNode("UI").AddChild(control, true);


		UpdatePositionAndColor(mousePosition);


	}

	private void UpdatePositionAndColor(Vector2 position)
	{
		var control = GetNode<Control>("UI/TowerPreview");
		var draggingTower = GetNode<Node2D>("UI/TowerPreview/DraggingTower");

		control.Position = position;


		Color color;

		// change color of tower
		if (CanPlaceTower)
		{
			color = new Color(0, 0.8f, 0, 1);
		}
		else
		{
			color = new Color(0.8f, 0, 0, 1);
		}

		if (draggingTower.Modulate.ToString() != color.ToString())
		{
			draggingTower.Modulate = color;
		}
	}


	private void UpdateTowerPlacementPreview()
	{
		var mousePosition = GetGlobalMousePosition();

		var currentTile = tileMap.LocalToMap(ToLocal(mousePosition));

		var tileData = tileMap.GetCellTileData(currentTile);

		// https://gamedevartisan.com/tutorials/godot-fundamentals/tilemaps-and-custom-data-layers
		// grass, base or road
		var tileType = tileData.GetCustomData("type").AsString();

		CanPlaceTower = tileType == "grass" && !GlobalData.Instance.TowerData.ContainsKey(currentTile);

		var tilePosition = ToGlobal(tileMap.MapToLocal(currentTile));


		UpdatePositionAndColor(tilePosition);
	}

	public void CancelBuildMode()
	{
		CanPlaceTower = false;
		IsInBuildMode = false;

		GetNode("UI/TowerPreview").QueueFree();


	}

	private void InitializeTowers()
	{
		foreach (var item in GlobalData.Instance.TowerData)
		{


			var towerScene = GD.Load<PackedScene>("res://base/towers/" + item.Value + ".tscn");
			var tower = towerScene.Instantiate<BasicTurret>();

			var tilePosition = ToGlobal(tileMap.MapToLocal(item.Key));

			GetNode("Towers").AddChild(tower);

			tower.Position = tilePosition;
			tower.built = true;
			tower.tileLocation = item.Key;
		}
	}

	private async void StartWave()
	{
		var waveInfo = GlobalData.Instance.WAVE_INFO[GlobalData.Instance.CurrentWave];

		GD.Print("Wave " + GlobalData.Instance.CurrentWave + " starts!");

		foreach (var unit in waveInfo.waveUnits)
		{
			var new_enemy = GD.Load<PackedScene>("res://base/enemies/" + unit.EnemyName + ".tscn").Instantiate();

			var Path2D = GetNode<Path2D>("Path2D");

			Path2D.AddChild(new_enemy);

			await Task.Delay((int)(unit.secondsDelayAfterEnemySpawning * 1000));
		}
	}

	private async void StartTutorial()
	{
		GetNode<Panel>("UI/HUD/Tutorial").Visible = true;

		// to load everything before pausing
		await Task.Delay(500);
		GetTree().Paused = true;
	}

	private void FinishTutorial()
	{
		GetTree().Paused = false;
		GetNode<Panel>("UI/HUD/Tutorial").Visible = false;
		GlobalData.Instance.TutorialSeenBase = true;

	}
}
