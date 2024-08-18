using Godot;
using Godot.Collections;

public partial class TurretConfig : GodotObject
{

    public string Name;
    public int Damage;
    public float ShotDelay;
    public float Range;
    public int Price;

    public TurretConfig(string Name, int Damage, float ShotDelay, float Range, int Price)
    {
        this.Name = Name;
        this.Damage = Damage;
        this.ShotDelay = ShotDelay;
        this.Range = Range;
        this.Price = Price;
    }
}

public partial class EnemyConfig : GodotObject
{

    public string Name;
    public int StartHP;
    public int DamageDealingToBase;
    public int Speed;

    public EnemyConfig(string Name, int StartHP, int DamageDealingToBase, int Speed)
    {
        this.Name = Name;
        this.StartHP = StartHP;
        this.DamageDealingToBase = DamageDealingToBase;
        this.Speed = Speed;
    }
}


public partial class WaveUnitConfig : GodotObject
{


    public string EnemyName;
    public float secondsDelayAfterEnemySpawning;

    public WaveUnitConfig(string EnemyName, float secondsDelayAfterEnemySpawning)
    {
        this.EnemyName = EnemyName;
        this.secondsDelayAfterEnemySpawning = secondsDelayAfterEnemySpawning;
    }
}



public partial class WaveConfig : GodotObject
{

    public int WaveNumber;
    public int SecondsUntilWaveStarts;
    public Array<WaveUnitConfig> waveUnits;

    public WaveConfig(int WaveNumber, int SecondsUntilWaveStarts, Array<WaveUnitConfig> waveUnits)
    {
        this.WaveNumber = WaveNumber;
        this.SecondsUntilWaveStarts = SecondsUntilWaveStarts;
        this.waveUnits = waveUnits;
    }
}
