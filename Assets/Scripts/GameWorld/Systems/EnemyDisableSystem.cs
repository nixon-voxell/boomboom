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
//[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyDisableSystem : ISystem
{
    private EntityManager entityManager;
    private EntityQuery enemyQuery;

    public void Initialize()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        enemyQuery = entityManager.CreateEntityQuery(typeof(EnemySpawnerSingleton));
    }

    public void Update()
    {
        // Check if the enemy spawning is complete
        bool enemySpawningComplete = enemyQuery.CalculateEntityCount() == 0;

        if (enemySpawningComplete)
        {
            // Disable all enemy entities
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            NativeArray<Entity> enemyEntities = entityManager.CreateEntityQuery(typeof(Prefab)).ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < enemyEntities.Length; i++)
            {
                commandBuffer.SetEnabled(enemyEntities[i], false);
            }

            commandBuffer.Playback(entityManager);
            commandBuffer.Dispose();

            enemyEntities.Dispose();
        }
    }
}


