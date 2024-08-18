using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	public int Resources { get; private set; } = 1000;

	public int SpaceGoal { get; private set; } = 100; // in chunks (* 1000)

	public int CurrentWave
	{
		get; private set;
	} = 0;

	public bool WaveIsActive = false;

	public int HP
	{
		get; private set;
	} = 100;


	public Dictionary<string, TurretConfig> TOWER_INFO;
	public Dictionary<string, EnemyConfig> ENEMY_INFO;

	public Dictionary<int, WaveConfig> WAVE_INFO;



	// store all towers, positions and upgrades
	private Dictionary<Vector2I, string> towerData;

	public Dictionary<Vector2I, string> TowerData { get { return towerData; } }

	private Timer NextWaveTimer;

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

		WAVE_INFO = new();
		var waveInfo1Units = new Array<WaveUnitConfig>();
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0.5f));
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 1));
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[1] = new WaveConfig(1, 5, waveInfo1Units);

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

	public void AddTower(string towerName, Vector2I cellLocation)
	{
		TowerData.Add(cellLocation, towerName);
	}

	public void RemoveTower(Vector2I cellLocation)
	{

		const float SELL_RATIO = 0.5f;

		var type = TowerData[cellLocation];
		TowerData.Remove(cellLocation);
		Resources += (int)(TOWER_INFO[type].Price * SELL_RATIO);
	}

	public void StartGame()
	{

		NextWaveTimer = new Timer() { OneShot = true, WaitTime = 1, Autostart = false };
		NextWaveTimer.Timeout += StartWave;

		AddChild(NextWaveTimer);

		StartWaveTimer();

	}

	public void StartWaveTimer()
	{
		CurrentWave++;

		GD.Print("Current wave: " + CurrentWave);
		var hasWave = WAVE_INFO.ContainsKey(CurrentWave);

		if (!hasWave)
		{
			GD.Print("you... won?");
			return;
		}

		var waveInfo = WAVE_INFO[CurrentWave];

		NextWaveTimer.WaitTime = waveInfo.SecondsUntilWaveStarts;


		NextWaveTimer.Start();
	}

	public async void StartWave()
	{
		WaveIsActive = true;

		var waveInfo = WAVE_INFO[CurrentWave];

		foreach (var unit in waveInfo.waveUnits)
		{
			// TODO trigger unit spawn
			GD.Print(unit.EnemyName);

			await Task.Delay((int)(unit.secondsDelayAfterEnemySpawning * 1000));
		}
	}

	public int GetSecondsLeftUntilNextWave()
	{
		return (int)NextWaveTimer.TimeLeft;
	}
}
