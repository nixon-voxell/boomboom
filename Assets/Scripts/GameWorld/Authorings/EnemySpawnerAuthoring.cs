using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour
{
    // values assigned to EnemySpawnerSingleton (ess) comp
    public float2 FieldDimensions;
    public GameObject EnemyPrefab;
    public uint RandomSeed;
    public float spawnInterval;

    public int PoolCount;
}

class EnemySpawnerBaker : Baker<EnemySpawner>   //bake mono values above to component values
{
    public override void Bake(EnemySpawner authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
        Entity prefabEnt = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        // Add spawner singleton with the spawn configs
        this.AddComponent<EnemySpawnerSingleton>(entity, new EnemySpawnerSingleton
        {
            FieldDimensions = authoring.FieldDimensions,
            SpawnInterval = authoring.spawnInterval,
            Randomizer = Random.CreateFromIndex(authoring.RandomSeed),
            // MaxEnemySpawnCount = authoring.PoolCount,
            // EnemyPrefab = prefabEnt,
        });

        // Add pool singleton that consists of the pool count and the entity prefab
        this.AddComponent<EnemyPoolSingleton>(entity, new EnemyPoolSingleton
        {
            PoolCount = authoring.PoolCount,
            Prefab = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
        });

        this.AddComponent<Pool.CurrentIndex>(entity);
        DynamicBuffer<Pool.Element> spawnerBuffer = this.AddBuffer<Pool.Element>(entity);
    }
}
