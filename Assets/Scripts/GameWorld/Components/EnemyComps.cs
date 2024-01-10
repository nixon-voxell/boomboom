using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public struct EnemySpawnerSingleton : IComponentData
{
    public float2 FieldDimensions;
    public float SpawnInterval;
    public Random Randomizer;
}

public readonly partial struct EnemySpawnerAspect : IAspect
{
    private readonly RefRO<LocalTransform> m_SpawnerLocalRO;
    // EnemySpawnerSingleton comp
    private readonly RefRW<EnemySpawnerSingleton> m_SpawnerCompRW;
    public float SpawnIntervalRW => m_SpawnerCompRW.ValueRW.SpawnInterval;

    #region Enemy Transform
    public LocalTransform GetRandomEnemyTransform()
    {
        return new LocalTransform
        {
            Position = this.GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f,
        };
    }

    private float3 GetRandomPosition()
    {
        ref EnemySpawnerSingleton essComp = ref m_SpawnerCompRW.ValueRW;

        float3 randomPosition = essComp.Randomizer.NextFloat3(m_MinCorner, m_MaxCorner);

        return randomPosition;
    }


    #region Calculation
    private float3 m_MinCorner => m_SpawnerLocalRO.ValueRO.Position - m_HalfDimensions;
    private float3 m_MaxCorner => m_SpawnerLocalRO.ValueRO.Position + m_HalfDimensions;

    private float3 m_HalfDimensions => new()
    {
        x = m_SpawnerCompRW.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = m_SpawnerCompRW.ValueRO.FieldDimensions.y * 0.5f,
    };

    #endregion
    #endregion
}

public struct EnemyPoolSingleton : IComponentData
{
    public int PoolCount;
    public Entity Prefab;
}

public struct EnemyInitialization : IComponentData { }
