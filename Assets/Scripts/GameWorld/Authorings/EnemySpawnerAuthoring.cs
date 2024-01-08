using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

class EnemySpawner : MonoBehaviour  //here we simply call it mono 
{
    public int PoolCount;
    public float2 FieldDimensions;  //here we simply call these mono data
    public int NumberEnemySpawn;
    public GameObject EnemyPrefab;
    public uint RandomSeed;
    public float spawnInterval;
}

class EnemySpawnerBaker : Baker<EnemySpawner>   //basically is to bring mono data above into ecs components
{
    public override void Bake(EnemySpawner authoring)   //authoring here = mono above
    {
        Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
        Entity enemyPrefab = this.GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

        /* EXPLANATION
         * entity: a newly created empty entity, and its tranformation is editable during runtime. Remember, this entity is EMPTY for now.
         * enemyPrefab: game object EnemyPrefab from mono above is converted into entity form, and being stored in this newly created entity.
        */

        this.AddComponent<EnemySpawnerSingleton>(entity, new EnemySpawnerSingleton
        //entity: this EMPTY entity will NOW be added with ecs component EnemySpawnerSingleton
        {
            FieldDimensions = authoring.FieldDimensions,    //this entity added ecs component values = mono above values
            NumberEnemySpawn = authoring.NumberEnemySpawn,
            EnemyPrefab = enemyPrefab,
            Randomizer = Random.CreateFromIndex(authoring.RandomSeed),
            SpawnInterval = authoring.spawnInterval,
        });
    }
}
