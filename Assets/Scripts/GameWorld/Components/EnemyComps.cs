using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public struct EnemySpawnerSingleton : IComponentData
{
    public float2 FieldDimensions;
    public Random Randomizer;
    public int ActiveEnemyCount;
}

public readonly partial struct EnemySpawnerAspect : IAspect
{
    public readonly RefRO<LocalTransform> Transform;
    public readonly RefRW<EnemySpawnerSingleton> Spawner;

    public int ActiveEnemyCountRO => this.Spawner.ValueRO.ActiveEnemyCount;
    public ref int ActiveEnemyCountRW => ref this.Spawner.ValueRW.ActiveEnemyCount;

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
        ref EnemySpawnerSingleton essComp = ref Spawner.ValueRW;

        float3 randomPosition = essComp.Randomizer.NextFloat3(m_MinCorner, m_MaxCorner);

        return randomPosition;
    }


    #region Calculation
    private float3 m_MinCorner => Transform.ValueRO.Position - m_HalfDimensions;
    private float3 m_MaxCorner => Transform.ValueRO.Position + m_HalfDimensions;

    private float3 m_HalfDimensions => new()
    {
        x = Spawner.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = Spawner.ValueRO.FieldDimensions.y * 0.5f,
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
