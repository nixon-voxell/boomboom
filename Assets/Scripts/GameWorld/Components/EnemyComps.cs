using Unity.Entities;
using Unity.Mathematics;

public struct EnemySpawnerSingleton : IComponentData    //spawner component
{
    public float2 FieldDimensions; 
    public float SpawnInterval;
    public Random Randomizer;
    public Entity EnemyPrefab;
    public int MaxEnemySpawnCount;
}
