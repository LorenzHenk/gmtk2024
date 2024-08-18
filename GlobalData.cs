using Godot;
using Godot.Collections;
using System;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	public int Resources { get; private set; } = 1000;

	public int CurrentWave
	{
		get; private set;
	}

	public int HP
	{
		get; private set;
	} = 100;


	// store all towers, positions and upgrades
	private Dictionary<Vector2, string> towerData;

	public Dictionary<Vector2, string> TowerData { get { return towerData; } }

	// TODO store all upgrades

	public override void _Ready()
	{
		// singleton can now be accessed everywhere via GlobalData.Instance
		Instance = this;
		towerData = new Dictionary<Vector2, string>();
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
}
