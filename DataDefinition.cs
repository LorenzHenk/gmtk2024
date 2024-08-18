public partial class TurretConfig : Godot.GodotObject
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

public partial class EnemyConfig : Godot.GodotObject
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
