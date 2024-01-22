using UnityEngine;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour
{
    [Tooltip("Enemies will spawn outside this radius.")]
    public float SpawnRadius = 40.0f;
    [Tooltip("Time interval between each enemy spawn.")]
    public float SpawnRate = 3.0f;
    public float Speed = 1.0f;
    public int PoolCount;
    public GameObject EnemyPrefab;
}

// Bake mono values above to component values
class EnemySpawnerBaker : Baker<EnemySpawner>
{
    public override void Bake(EnemySpawner authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
        Entity prefabEnt = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        // Add spawner singleton with the spawn configs
        this.AddComponent<EnemySpawnerSingleton>(entity, new EnemySpawnerSingleton
        {
            Radius = authoring.SpawnRadius,
            Randomizer = Random.CreateFromIndex(1),
            PoolCount = authoring.PoolCount,
            Prefab = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
        });

        this.AddBuffer<DisabledEnemy>(entity);

        this.AddComponent<Timer>(entity, new Timer
        {
            TotalTime = authoring.SpawnRate,
            // Make sure to have enemies spawned in the first round.
            ElapsedTime = authoring.SpawnRate + 1.0f,
        });

        this.AddComponent<EnemyProgressionSingleton>(entity, new EnemyProgressionSingleton
        {
            SpawnRate = authoring.SpawnRate,
            Speed = authoring.Speed,
        });
    }
}
