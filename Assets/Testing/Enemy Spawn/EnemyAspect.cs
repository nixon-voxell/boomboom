using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct CubeAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRW<LocalTransform> m_Local;

    private readonly RefRO<EnemyComponent> m_EnemyComponent;
    private readonly RefRW<EnemyRandom> m_EnemyRandom;

    public int NumberEnemySpawn => m_EnemyComponent.ValueRO.NumberEnemySpawn;
    public Entity EnemyPrefab => m_EnemyComponent.ValueRO.EnemyPrefab;

    public LocalTransform GetRandomEnemyTransform()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f,
        };
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;

        randomPosition = m_EnemyRandom.ValueRW.Value.NextFloat3(minCorner, maxCorner);

        return randomPosition;
    }
    
    private float3 minCorner => m_Local.ValueRW.Position - HalfDimensions;
    private float3 maxCorner => m_Local.ValueRW.Position + HalfDimensions;
    private float3 HalfDimensions => new()
    {
        x = m_EnemyComponent.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = m_EnemyComponent.ValueRO.FieldDimensions.y * 0.5f,
    };
}
