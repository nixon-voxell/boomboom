using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct cubeSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<cubeComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var cubeEntity = SystemAPI.GetSingletonEntity<cubeComponent>();
        var cubeAsp = SystemAPI.GetAspect<cubeAspect>(cubeEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var i = 0; i < cubeAsp.numberCubeSpawn; i++)
        {
            ecb.Instantiate(cubeAsp.cubePrefab);
        }

        ecb.Playback(state.EntityManager);
    }
}
