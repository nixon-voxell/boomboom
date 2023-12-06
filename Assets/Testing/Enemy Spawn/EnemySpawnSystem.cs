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
public partial struct CubeSpawnSystem : ISystem
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
        var cubeEntity = SystemAPI.GetSingletonEntity<EnemyComponent>();
        var cubeAsp = SystemAPI.GetAspect<CubeAspect>(cubeEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var i = 0; i < cubeAsp.NumberEnemySpawn; i++)
        {
            var newCube = ecb.Instantiate(cubeAsp.EnemyPrefab);
            var newCubeTransform = cubeAsp.GetRandomEnemyTransform();
            ecb.SetComponent<LocalTransform>(newCube, newCubeTransform);
        }

        ecb.Playback(state.EntityManager);
    }
}
