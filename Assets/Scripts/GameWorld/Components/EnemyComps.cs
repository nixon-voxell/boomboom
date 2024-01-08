using Unity.Entities;
using Unity.Mathematics;

public struct EnemySpawnerSingleton : IComponentData
{
    public float2 FieldDimensions; 
    public int NumberEnemySpawn;
    public float SpawnInterval;
    public Random Randomizer;
    public Entity EnemyPrefab;
}
