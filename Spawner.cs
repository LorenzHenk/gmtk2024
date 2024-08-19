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
    public float SpaceBetweenObstacles = 400.0f; // distance between objetcs

	[Export]
	public float HeightScalingFactor = 5000f; // height/spawnrate balance factor

    public Vector2 ChunkSize; 

    private float ChunkBorderPadding = 100f;

    private Player player;
    private Dictionary<Vector2, List<Node2D>> chunks = new Dictionary<Vector2, List<Node2D>>();

	public override void _Ready()
	{
        base._Ready();

		if(Scenes.Count == 0)
		{
			GD.PrintErr("Scene/s is null!");
			return;
		}

        ChunkSize = new Vector2(GlobalData.Instance.ChunkSize, GlobalData.Instance.ChunkSize);
		player = GetNode<Player>("../Player");
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
        return new Vector2(Mathf.Floor(position.X / GlobalData.Instance.ChunkSize), Mathf.Floor(position.Y / GlobalData.Instance.ChunkSize));
    }

    private void SpawnChunk(Vector2 chunkPosition)
    {
        List<Node2D> chunk = new List<Node2D>();
        float spawnMultiplier = 1.0f + (Mathf.Abs(player.GlobalPosition.Y) / HeightScalingFactor);
        float spawnRate = BaseSpawnRate * spawnMultiplier;
        if(spawnRate > 10) spawnRate = 10;
        
        for (int i = 0; i < (int)spawnRate; i++)
        {
            Vector2 spawnPosition = GetSpawnPositionInChunk(chunkPosition);

            if (IsPositionValid(spawnPosition, chunk))
            {
                var scene = (PackedScene)Scenes[Math.Abs((int)GD.Randi()) % Scenes.Count];
                if(Math.Abs(player.Position.Y) < 2 * GlobalData.Instance.ChunkSize)
                {
                    scene = (PackedScene)Scenes[0]; // spawn only astros
                }

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
        float spawnX = chunkPosition.X * GlobalData.Instance.ChunkSize + GD.Randf() * (GlobalData.Instance.ChunkSize - 2 * ChunkBorderPadding) + ChunkBorderPadding;
        float spawnY = chunkPosition.Y * GlobalData.Instance.ChunkSize + GD.Randf() * (GlobalData.Instance.ChunkSize - 2 * ChunkBorderPadding) + ChunkBorderPadding;
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
		if(-GlobalData.Instance.ChunkSize < position.X && 
           position.X < GlobalData.Instance.ChunkSize &&
		   -GlobalData.Instance.ChunkSize < position.Y && 
           position.Y < GlobalData.Instance.ChunkSize)
		{
			return false;
		}

		foreach (var gameObject in chunk)
		{
			if (gameObject != null && 
				!gameObject.IsQueuedForDeletion() && 
				gameObject.Position.DistanceTo(position) <  SpaceBetweenObstacles)
			{
				return false;
			}
		}

		return true;
	}
}

