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

    public int MaxEnemySpawnCount;
}

class EnemySpawnerBaker : Baker<EnemySpawner>   //bake mono values above to component values
{
    public override void Bake(EnemySpawner authoring)   
    {
        Entity spawnerEnt = GetEntity(TransformUsageFlags.Dynamic);
        Entity prefabEnt = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        EnemySpawnerSingleton essComp = new EnemySpawnerSingleton
        {
            EnemyPrefab = prefabEnt,
            FieldDimensions = authoring.FieldDimensions,
            SpawnInterval = authoring.spawnInterval,
            Randomizer = Random.CreateFromIndex(authoring.RandomSeed),  
            MaxEnemySpawnCount = authoring.MaxEnemySpawnCount,
        };

        AddComponent<EnemySpawnerSingleton>(spawnerEnt, essComp);
        AddComponent<Pool.CurrentIndex>(spawnerEnt);

        DynamicBuffer<Pool.Element> spawnerBuffer = AddBuffer<Pool.Element>(spawnerEnt);  



        /*
        for (int e = 0; e < authoring.MaxEnemySpawnCount; e++)   //e: current enemyEnt count
        {
            Entity enemyEnt = CreateAdditionalEntity(TransformUsageFlags.Dynamic, entityName: "Enemy");    //create empty entity
            AddComponent<Disabled>(enemyEnt);
            
            Pool.Element element = new Pool.Element { Entity = enemyEnt };   //create new element & assign

            spawnerBuffer.Add(element);
        }
        */
    }
}
