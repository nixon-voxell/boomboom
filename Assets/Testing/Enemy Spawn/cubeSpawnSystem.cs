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
            var newCube = ecb.Instantiate(cubeAsp.cubePrefab);
            var newCubeTransform = cubeAsp.GetRandomCubeTransform();
            ecb.SetComponent<LocalTransform>(newCube, newCubeTransform);
        }

        ecb.Playback(state.EntityManager);
    }
}
