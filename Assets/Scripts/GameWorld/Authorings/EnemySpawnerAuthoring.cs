using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberEnemySpawn;
    public GameObject EnemyPrefab;
    public uint RandomSeed;
}

class EnemySpawnerBaker : Baker<EnemySpawner>
{
    public override void Bake(EnemySpawner authoring)
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
        Entity enemyPrefab = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        this.AddComponent<EnemySpawnerSingleton>(entity, new EnemySpawnerSingleton
        {
            FieldDimensions = authoring.FieldDimensions,
            NumberEnemySpawn = authoring.NumberEnemySpawn,
            EnemyPrefab = enemyPrefab,
            Randomizer = Random.CreateFromIndex(authoring.RandomSeed),
        });
    }
}
