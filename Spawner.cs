using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
    [Export]
    public Godot.Collections.Array Scenes = new Godot.Collections.Array();

    [Export]
    public float BaseSpawnRate = 1f; // minimum object in chunk

    [Export]
    public float SpaceBetweenObstacles = 500.0f; // distance between objetcs

	[Export]
	public float HeightScalingFactor = 10000f; // height/spawnrate balance factor

    [Export]
    public Vector2 ChunkSize = new Vector2(1000, 1000);

    private Player player;
    private Camera2D camera;
    private Dictionary<Vector2, List<Node2D>> chunks = new Dictionary<Vector2, List<Node2D>>();

	public override void _Ready()
	{
        base._Ready();
        
		if(Scenes.Count == 0)
		{
			GD.PrintErr("Scene/s is null!");
			return;
		}

		player = GetNode<Player>("../Player");
		camera = player.GetNode<Camera2D>("Camera2D");
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(Scenes.Count == 0)
		{
			GD.PrintErr("Scene/s is null!");
			return;
		}

        UpdateChunks();
    }

	public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        foreach (var chunk in chunks)
        {
            chunk.Value.RemoveAll(obj => obj == null || obj.IsQueuedForDeletion());
        }
    }

    private void UpdateChunks()
    {
        Vector2 playerChunk = GetChunkPosition(player.GlobalPosition);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 chunkPos = playerChunk + new Vector2(x, y);
                if (!chunks.ContainsKey(chunkPos))
                {
                    SpawnChunk(chunkPos);
                }
            }
        }
    }

    private Vector2 GetChunkPosition(Vector2 position)
    {
        return new Vector2(Mathf.Floor(position.X / ChunkSize.X), Mathf.Floor(position.Y / ChunkSize.Y));
    }

    private void SpawnChunk(Vector2 chunkPosition)
    {
        List<Node2D> chunk = new List<Node2D>();
        float spawnMultiplier = 1.0f + (Mathf.Abs(player.GlobalPosition.Y) / HeightScalingFactor);
        float spawnRate = BaseSpawnRate * spawnMultiplier;

        for (int i = 0; i < spawnRate * 1; i++)
        {
            Vector2 spawnPosition = GetSpawnPositionInChunk(chunkPosition);

            if (IsPositionValid(spawnPosition, chunk))
            {
				var scene = (PackedScene)Scenes[Math.Abs((int)GD.Randi()) % Scenes.Count];
                var gameObject = (Node2D)scene.Instantiate();
                gameObject.Position = spawnPosition;         
				gameObject.TreeExited += () => OnGameObjectDeleted(gameObject);
				AddChild(gameObject);
                chunk.Add(gameObject);
            }
        }

        chunks[chunkPosition] = chunk;
    }

    private Vector2 GetSpawnPositionInChunk(Vector2 chunkPosition)
    {
        float spawnX = chunkPosition.X * ChunkSize.X + GD.Randf() * ChunkSize.X;
        float spawnY = chunkPosition.Y * ChunkSize.Y + GD.Randf() * ChunkSize.Y;
        return new Vector2(spawnX, spawnY);
    }

    private void OnGameObjectDeleted(Node2D gameObject)
    {
        Vector2 chunkPos = GetChunkPosition(gameObject.Position);
        if (chunks.ContainsKey(chunkPos))
        {
            chunks[chunkPos].Remove(gameObject);
        }
    }

	private bool IsPositionValid(Vector2 position, List<Node2D> chunk)
	{
		// basically first "chunk" should be obstacle-free
		if(-1000 < position.X && position.X < 1000 &&
		   -1000 < position.Y && position.Y < 1000)
		{
			return false;
		}

		foreach (var gameObject in chunk)
		{
			if (gameObject != null && 
				!gameObject.IsQueuedForDeletion() && 
				gameObject.Position.DistanceTo(position) < SpaceBetweenObstacles)
			{
				return false;
			}
		}

		Vector2 playerChunk = GetChunkPosition(player.GlobalPosition);
		foreach (var offset in new Vector2[] { new Vector2(-1, -1), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1) })
		{
			Vector2 adjacentChunkPos = playerChunk + offset;
			if (chunks.ContainsKey(adjacentChunkPos))
			{
				foreach (var gameObject in chunks[adjacentChunkPos])
				{
					if (gameObject != null && 
						!gameObject.IsQueuedForDeletion() && 
						gameObject.Position.DistanceTo(position) < SpaceBetweenObstacles)
					{
						return false;
					}
				}
			}
		}

		return true;
	}
}

