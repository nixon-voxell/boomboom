using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]  //system runs on every frame
public partial struct EnemyDisableSystem : ISystem
{
    private EntityManager entityManager;
    private EntityQuery enemyQuery;
    private DynamicBuffer<Entity> enemyBuffer;

    public void Initialize(ref SystemState state)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        enemyQuery = entityManager.CreateEntityQuery(typeof(EnemySpawnerSingleton));
        enemyBuffer = new DynamicBuffer<Entity>();
    }

    public void Update(ref SystemState state)
    {
        // Count the number of entities with EnemySpawnerSingleton component
        int entityCount = entityManager.CreateEntityQuery(typeof(EnemySpawnerSingleton)).CalculateEntityCount();

        // Debug the entity count
        Debug.Log("Number of entities with EnemySpawnerSingleton: " + entityCount);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }
}


