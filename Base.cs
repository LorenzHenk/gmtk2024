using Godot;
using System;
using System.IO;

public partial class Base : Node2D
{

	private bool IsInBuildMode { get; set; }

	private bool CanPlaceTower { get; set; }


	private TileMapLayer tileMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap = GetNode<TileMapLayer>("TileMapLayer");
		// TODO load data from GlobalData.Instance.TowerData
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsInBuildMode)
		{
			UpdateTowerPlacementPreview();
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!IsInBuildMode)
		{
			return;
		}

		if (@event is InputEventMouse)
		{
			var evt = (InputEventMouse)@event;
			if (evt.ButtonMask == MouseButtonMask.Left && @event.IsPressed())
			{
				var globalClicked = GetGlobalMousePosition();

				var currentTile =
					tileMap.LocalToMap(ToLocal(globalClicked));

				var tilePosition = ToGlobal(tileMap.MapToLocal(currentTile));

				GD.Print(currentTile, tilePosition);


				if (CanPlaceTower)
				{
					GlobalData.Instance.AddTower("BasicTurret", currentTile);

					var tower = GetNode<Node2D>("UI/TowerPreview/DraggingTower");
					GetNode("UI/TowerPreview").RemoveChild(tower);

					GetNode("Towers").AddChild(tower);

					tower.Modulate = new Color(1, 1, 1, 1);
					tower.Position = tilePosition;

					CancelBuildMode();
				}

			}
		}
	}

	public void NextWaveTimerTimeout()
	{
		GlobalData.Instance.NextWave();
		GD.Print("Wave " + GlobalData.Instance.CurrentWave + " starts!");

		string enemyName = "Simp";
		var new_enemy = GD.Load<PackedScene>("res://base/enemies/" + enemyName + ".tscn").Instantiate();

		var Path2D = GetNode<Path2D>("Path2D");

		Path2D.AddChild(new_enemy);

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

		GD.Print(currentTile);

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
}
