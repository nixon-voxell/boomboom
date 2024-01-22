using UnityEngine;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

class EnemySpawnerAuthoring : MonoBehaviour
{
    [Header("Pool Config")]
    public GameObject EnemyPrefab;
    public int PoolCount;

    [Space]
    [Header("Spawn Config")]
    [Tooltip("Enemies will spawn outside this radius.")]
    public float SpawnRadius = 20.0f;
    [Header("Spawn Rate")]
    [Tooltip("Time interval between each enemy spawn.")]
    public float StartSpawnRate = 3.0f;
    public float EndSpawnRate = 0.3f;
    [Header("Speed")]
    public float StartSpeed = 1.0f;
    public float EndSpeed = 8.0f;
    [Tooltip("The time (in seconds) where all values will reach their ending values.")]
    public float EndTime = 30.0f * 60.0f;
}

// Bake mono values above to component values
class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
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
            TotalTime = authoring.StartSpawnRate,
            // Make sure to have enemies spawned in the first round.
            ElapsedTime = 0.0f,
        });

        this.AddComponent<EnemyProgressionSingleton>(entity, new EnemyProgressionSingleton
        {
            StartSpawnRate = authoring.StartSpawnRate,
            EndSpawnRate = authoring.EndSpawnRate,
            StartSpeed = authoring.StartSpeed,
            EndSpeed = authoring.EndSpeed,

            EndTime = authoring.EndTime,

            CurrSpeed = authoring.StartSpeed,
            CurrSpawnRate = authoring.StartSpawnRate,
        });
    }
}
