using Godot;
using Godot.Collections;
using System;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	public int Resources { get; private set; } = 1000;

	public int SpaceGoal {get; private set;} = 100; // in chunks (* 1000)

	public int CurrentWave
	{
		get; private set;
	}

	public int HP
	{
		get; private set;
	} = 100;


	public Dictionary<string, TurretConfig> TOWER_INFO;
	public Dictionary<string, EnemyConfig> ENEMY_INFO;

	// store all towers, positions and upgrades
	private Dictionary<Vector2, string> towerData;

	public Dictionary<Vector2, string> TowerData { get { return towerData; } }

	// TODO store all upgrades

	public override void _Ready()
	{
		// singleton can now be accessed everywhere via GlobalData.Instance
		Instance = this;
		towerData = new();

		TOWER_INFO = new();
		TOWER_INFO["BasicTurret"] = new TurretConfig("BasicTurret", 1, 1f, 5f, 100);

		ENEMY_INFO = new();
		ENEMY_INFO["Simp"] = new EnemyConfig("Simp", 2, 1, 250);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// TODO store what is bought
	public void Buy(int cost)
	{
		if (!CanBuy(cost))
		{
			throw new Exception("Not enough money");
		}

		Resources -= cost;
	}

	public bool CanBuy(int cost)
	{
		return Resources >= cost;
	}

	public void AddResources(int amount)
	{
		Resources += amount;
	}

	public void NextWave()
	{
		CurrentWave++;
	}

	public void TakeDamage(int damage)
	{
		HP -= damage;
		if (HP < 0)
		{
			HP = 0;
		}
	}

	public void AddTower(string towerName, Vector2 cellLocation)
	{
		TowerData.Add(cellLocation, towerName);
	}

	public void RemoveTower(Vector2 cellLocation)
	{

		const float SELL_RATIO = 0.5f;

		var type = TowerData[cellLocation];
		TowerData.Remove(cellLocation);
		Resources += (int)(TOWER_INFO[type].Price * SELL_RATIO);
	}
}
