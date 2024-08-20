using Godot;
using Godot.Collections;
using System;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	[Signal]
	public delegate void WaveStartedEventHandler();

	public int Resources { get; private set; } = 100;

	public int ChunkSize = 1000;

	public int SpaceGoal { get; private set; } = 50; // in chunks (* 1000)

	public int CurrentWave
	{
		get; private set;
	} = 0;

	public bool WaveIsActive = false;

	public int HP
	{
		get; private set;
	} = 100;

	public int WaveCount { get; set; } = 0;
	public int ResourcesGathered { get; set; } = 0;
	public int ResourceAmount = 100;

	public Dictionary<string, TurretConfig> TOWER_INFO;
	public Dictionary<string, EnemyConfig> ENEMY_INFO;
	public Dictionary<int, WaveConfig> WAVE_INFO;

	// store all towers, positions and upgrades
	private Dictionary<Vector2I, string> towerData;
	public Dictionary<Vector2I, string> TowerData { get { return towerData; } }
	private Timer NextWaveTimer;

	// TODO store all upgrades

	public bool TutorialSeenBase { get; set; } = false;
	public bool TutorialSeenSpace { get; set; } = false;

	public override void _Ready()
	{
		// singleton can now be accessed everywhere via GlobalData.Instance
		Instance = this;
		towerData = new();

		TOWER_INFO = new();
		TOWER_INFO["BasicTurret"] = new TurretConfig("BasicTurret", 5, 1f, 5f, 100);

		ENEMY_INFO = new();
		ENEMY_INFO["Simp"] = new EnemyConfig("Simp", 10, 1, 250);

		WAVE_INFO = new();
		var waveInfo1Units = new Array<WaveUnitConfig>();
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0.5f));
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0.5f));
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0.5f));
		waveInfo1Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[1] = new WaveConfig(1, 60, waveInfo1Units);

		var waveInfo2Units = new Array<WaveUnitConfig>();
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0.3f));
		waveInfo2Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[2] = new WaveConfig(2, 120, waveInfo2Units);

		var waveInfo3Units = new Array<WaveUnitConfig>();
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0.25f));
		waveInfo3Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[3] = new WaveConfig(3, 180, waveInfo3Units);

		var waveInfo4Units = new Array<WaveUnitConfig>();
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0.1f));
		waveInfo4Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[4] = new WaveConfig(4, 240, waveInfo4Units);

		var waveInfo5Units = new Array<WaveUnitConfig>();
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0.05f));
		waveInfo5Units.Add(new WaveUnitConfig("Simp", 0));
		WAVE_INFO[5] = new WaveConfig(5, 300, waveInfo5Units);

		WAVE_INFO[6] = new WaveConfig(6, 300, waveInfo5Units);
		WAVE_INFO[7] = new WaveConfig(7, 300, waveInfo5Units);
		WAVE_INFO[8] = new WaveConfig(8, 300, waveInfo5Units);
		WAVE_INFO[9] = new WaveConfig(9, 300, waveInfo5Units);
		
		
		var waveInfo10Units = new Array<WaveUnitConfig>();
		for (int i = 0; i < 150; i++) {
			waveInfo10Units.Add(new WaveUnitConfig("Simp", 0));
		}
		WAVE_INFO[10] = new WaveConfig(10, 600, waveInfo10Units);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

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

	public void EndWave()
	{
		WaveIsActive = false;
		WaveCount++;
		StartWaveTimer();
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

	public void StartWave()
	{
		WaveIsActive = true;

		EmitSignal(SignalName.WaveStarted);
	}

	public int GetSecondsLeftUntilNextWave()
	{
		return (int)NextWaveTimer.TimeLeft;
	}
}
