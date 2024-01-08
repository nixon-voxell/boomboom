using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour  
{
    // values assigned to EnemySpawnerSingleton (ess) comp
    public float2 FieldDimensions;  
    public int NumberEnemySpawn;
    public GameObject EnemyPrefab;
    public uint RandomSeed;
    public float spawnInterval;

    public int PoolCount;
}

class EnemySpawnerBaker : Baker<EnemySpawner>   //basically is to bring mono data above into ecs components
{
    public override void Bake(EnemySpawner authoring)   
    {
        Entity enemyEnt = GetEntity(TransformUsageFlags.Dynamic);

        EnemySpawnerSingleton essComp = new EnemySpawnerSingleton 
        { 
            FieldDimensions = authoring.FieldDimensions,
            NumberEnemySpawn = authoring.NumberEnemySpawn,
            SpawnInterval = authoring.spawnInterval,
            Randomizer = Random.CreateFromIndex(authoring.RandomSeed),
            //EnemyPrefab, value not yet assigned.
        };

        AddComponent<EnemySpawnerSingleton>(enemyEnt, essComp);
        AddComponent<Pool.CurrentIndex>(enemyEnt);


        DynamicBuffer<Pool.Element> enemiesBuffer = AddBuffer<Pool.Element>(enemyEnt);

        for (int e = 0; e < authoring.PoolCount; e++)   //e: current enemyEnt count
        {
            Entity enemyPrefab = this.CreateAdditionalEntity(TransformUsageFlags.Dynamic);    //create empty entity

            Pool.Element element = new Pool.Element { Entity = enemyPrefab };
            enemiesBuffer.Add(element);
        }
    }
}
