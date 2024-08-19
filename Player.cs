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

    public int CurrentResources = 0;
    public bool Damaged = false;

    private int goal;
    private const int DefaultPlayerDamage = 1;
    private const float RotationThreshold = 50f;
    private Camera2D _camera;
    private Vector2 _velocity = Vector2.Zero;
    private Timer damageTimer;
    private AnimatedSprite2D burst;
    private bool soundPlaying = false;
    private Label DistanceLabel;
    private ProgressBar DistanceProgressBar;
    private int startDistanceY;
    private Label ResourceLabel;

    public override void _Ready()
    {
        base._Ready();

        goal = GlobalData.Instance.ChunkSize * GlobalData.Instance.SpaceGoal;

        damageTimer = new Timer
        {
            WaitTime = 1.0f,  //in seconds
            OneShot = true
        };
        damageTimer.Connect("timeout", new Callable(this, nameof(OnDamageTimerTimeout)));
        AddChild(damageTimer);

        burst = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        DistanceLabel = GetNode<Label>("HUD/HB/DistanceLabel");

        DistanceProgressBar = GetNode<ProgressBar>("./HUD/ProgressBar");
        DistanceProgressBar.MaxValue = goal;

        ResourceLabel = GetNode<Label>("HUD/HB/Res/ResourceLabel");

        startDistanceY = (int)GlobalPosition.Y;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var distance = GetDistance();

        DistanceLabel.Text = "Distance: " + (distance / 100).ToString();
        DistanceProgressBar.Value = distance;
        ResourceLabel.Text = "Resources: " + CurrentResources;
        var landButton = GetNodeOrNull<Button>("HUD/LandButton");
        if (landButton != null)
        {
            landButton.Visible = distance < 500;
        }

        var homeButton = GetNodeOrNull<Button>("HUD/HomeButton");
        if (homeButton != null)
        {
            homeButton.Visible = distance  >= goal + GlobalData.Instance.ChunkSize * 1.0f;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GetInput((float)delta);
        var collision = MoveAndCollide(Velocity * (float)delta);
        
        if (Position.Y > 500)
        {
            Position = new Vector2(Position.X, 500);
        }

        if(Position.Y <= -goal - GlobalData.Instance.ChunkSize * 0.8f)
        {
            Position = new Vector2(Position.X, -goal - GlobalData.Instance.ChunkSize * 0.8f);
        }

        if (collision != null)
        {
            Node collidedObject = collision.GetCollider() as Node;

            if (!CheckPlayerHealth())
            {
                var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");
                sceneHandler.BackToBase();
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
                CurrentResources += GlobalData.Instance.ResourceAmount;
                GlobalData.Instance.ResourcesGathered += GlobalData.Instance.ResourceAmount;
            }
        }
    }


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

        CurrentResources = CurrentResources <= 50 ? 0 : CurrentResources - 50;
        Health -= amount;
    }

    private bool CheckPlayerHealth()
    {
        if (Health <= 0)
        {
            return false;
        }

        return true;
    }

    private void OnDamageTimerTimeout()
    {
        Damaged = false;
    }

    private int GetDistance()
    {
        return startDistanceY - (int)GlobalPosition.Y;
    }

    private void OnHomeButtonHandler()
    {
        var sceneHandler = GetNodeOrNull<SceneHandler>("/root/SceneHandler");
        sceneHandler.EndMenuHandler();
    }
}