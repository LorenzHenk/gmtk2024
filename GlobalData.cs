using Godot;
using System;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	public int Resources { get; private set; }

	// TODO store all upgrades

	public override void _Ready()
	{
		// singleton can now be accessed everywhere via GlobalData.Instance
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// TODO store what is bought
	public bool Buy(int cost)
	{
		if (!CanBuy(cost))
		{
			return false;
		}

		Resources -= cost;

		return true;
	}

	public bool CanBuy(int cost)
	{
		return Resources >= cost;
	}

	public void AddResources(int amount)
	{
		Resources += amount;
	}
}
