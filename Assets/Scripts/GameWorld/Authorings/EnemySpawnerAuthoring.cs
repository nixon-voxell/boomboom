using UnityEngine;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour
{
    [Tooltip("Enemies will spawn outside this radius.")]
    public float SpawnRadius = 40.0f;
    [Tooltip("Time between each round. New enemies will be spawned after the end of a round.")]
    public float RoundInterval;
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
            TotalTime = authoring.RoundInterval,
            // Make sure to have enemies spawned in the first round.
            ElapsedTime = authoring.RoundInterval + 1.0f,
        });
    }
}
