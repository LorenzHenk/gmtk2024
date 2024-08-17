using Godot;

public partial class PlayerMovement : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 800;

    [Export]
    public float Inertia { get; set; } = 1000f;

    [Export]
    public float RotationSpeed { get; set; } = 50f;

    private Vector2 _velocity = Vector2.Zero;
    private const float RotationTreshold = 50f;

    public void GetInput(float delta)
    {
        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

        if (inputDirection != Vector2.Zero)
        {
            _velocity = _velocity.MoveToward(inputDirection * Speed, Inertia * delta);
        }
        else
        {
            _velocity = _velocity.MoveToward(Vector2.Zero, Inertia * delta);
        }

        // apply Velocity
        Velocity = _velocity;

        // Rotate ship in towards moving direction
        if (_velocity.Length() > RotationTreshold)
        {
            float targetRotation = _velocity.Angle();
            Rotation = Mathf.LerpAngle(Rotation, targetRotation, RotationSpeed * (float)delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput((float)delta);
        MoveAndSlide();
    }
}
