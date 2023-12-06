using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var enemyEntity = SystemAPI.GetSingletonEntity<EnemyComponent>();
        var enemyAsp = SystemAPI.GetAspect<CubeAspect>(enemyEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var i = 0; i < enemyAsp.NumberEnemySpawn; i++)
        {
            var newCube = ecb.Instantiate(enemyAsp.EnemyPrefab);
            var newCubeTransform = enemyAsp.GetRandomEnemyTransform();
            ecb.SetComponent<LocalTransform>(newCube, newCubeTransform);
        }

        ecb.Playback(state.EntityManager);
    }
}
