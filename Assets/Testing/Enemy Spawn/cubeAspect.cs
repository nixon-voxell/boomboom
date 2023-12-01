using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct cubeAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRW<LocalTransform> _local;

    private readonly RefRO<cubeComponent> _cubeComponent;
    private readonly RefRW<cubeRandom> _cubeRandom;

    public int numberCubeSpawn => _cubeComponent.ValueRO.NumberCubeSpawn;
    public Entity cubePrefab => _cubeComponent.ValueRO.CubePrefab;

    public LocalTransform GetRandomCubeTransform()
    {
        return new LocalTransform
        {
            Position = getRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f,
        };
    }

    private float3 getRandomPosition()
    {
        float3 randomPosition;

        randomPosition = _cubeRandom.ValueRW.Value.NextFloat3(minCorner, maxCorner);

        return randomPosition;
    }
    
    private float3 minCorner => _local.ValueRW.Position - HalfDimensions;
    private float3 maxCorner => _local.ValueRW.Position + HalfDimensions;
    private float3 HalfDimensions => new()
    {
        x = _cubeComponent.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = _cubeComponent.ValueRO.FieldDimensions.y * 0.5f,
    };
}
