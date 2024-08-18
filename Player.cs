using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 800;

    [Export]
    public float Inertia { get; set; } = 1000f;

    [Export]
    public float RotationSpeed { get; set; } = 50f;

    [Export]
    public int Health { get; set; } = 3;

    [Export]
    public AudioStreamPlayer thrusterSound;

    private const int DefaultPlayerDamage = 1;
    private const float RotationThreshold = 50f;
    private Camera2D _camera;
    private Vector2 _velocity = Vector2.Zero;
    private Timer damageTimer;
    private AnimatedSprite2D burst;
    public bool Damaged = false;
    private bool soundPlaying = false;

    private RichTextLabel DistanceLabel;

    private int startDistanceY;

    public void GetInput(float delta)
    {
        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

        if (inputDirection != Vector2.Zero)
        {
            burst.Play();
            burst.Visible = true;
            if (!soundPlaying || !thrusterSound.Playing)
            {
                soundPlaying = true;
                thrusterSound.Play();
            }

            _velocity = _velocity.MoveToward(inputDirection * Speed, Inertia * delta);
        }
        else
        {
            burst.Stop();
            burst.Visible = false;
            if (soundPlaying)
            {
                soundPlaying = false;
                thrusterSound.Stop();
            }
            _velocity = _velocity.MoveToward(Vector2.Zero, Inertia * delta);
        }

        // apply Velocity
        Velocity = _velocity;

        // Rotate ship towards moving direction
        if (_velocity.Length() > RotationThreshold)
        {
            float targetRotation = _velocity.Angle();
            Rotation = Mathf.LerpAngle(Rotation, targetRotation, RotationSpeed * (float)delta);
        }
    }

    public void Damage(int amount)
    {
        if (amount <= -1)
        {
            amount = DefaultPlayerDamage;
        }

        Health -= amount;
    }

    private bool CheckPlayerHealth()
    {
        if (Health <= 0)
        {
            // TODO: restart level / game?
            GD.Print("Player died");
            return false;
        }

        return true;
    }

    private void OnDamageTimerTimeout()
    {
        Damaged = false;
    }

    public override void _Ready()
    {
        base._Ready();

        damageTimer = new Timer
        {
            WaitTime = 1.0f,  //in seconds
            OneShot = true
        };
        damageTimer.Connect("timeout", new Callable(this, nameof(OnDamageTimerTimeout)));
        AddChild(damageTimer);

        burst = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        DistanceLabel = GetNode<RichTextLabel>("HUD/DistanceLabel");

        startDistanceY = (int)GlobalPosition.Y;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var distance = GetDistance();

        DistanceLabel.Text = distance.ToString();



        var landButton = GetNodeOrNull<Button>("HUD/LandButton");
        if (landButton != null)
        {
            landButton.Visible = distance < 15;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GetInput((float)delta);
        var collision = MoveAndCollide(Velocity * (float)delta);

        if (collision != null)
        {
            Node collidedObject = collision.GetCollider() as Node;

            if (!CheckPlayerHealth())
            {
                return;
            }
            else if (collidedObject.IsInGroup("Asteroids") && Damaged)
            {
                return;
            }
            else if (collidedObject.IsInGroup("Asteroids") && !Damaged)
            {
                ((Asteroid)collidedObject)?.EmitSignal(Asteroid.SignalName.PlayerCollision);
                ((HUD)GetNode<CanvasLayer>("HUD"))?.EmitSignal(HUD.SignalName.PlayerDamage);
                Damage(1);
                Damaged = true;
                damageTimer.Start();
            }
            else if (collidedObject.IsInGroup("Resources"))
            {
                ((Resource)collidedObject)?.EmitSignal(Resource.SignalName.PlayerCollision);
            }
        }
    }

    private int GetDistance()
    {
        return (startDistanceY - (int)GlobalPosition.Y) / 100;
    }
}