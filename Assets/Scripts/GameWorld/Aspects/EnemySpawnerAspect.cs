using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct EnemySpawnerAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRO<LocalTransform> m_Local;
    private readonly RefRW<EnemySpawnerSingleton> m_EnemyComponent;

    public int NumberEnemySpawn => m_EnemyComponent.ValueRO.NumberEnemySpawn;
    public Entity EnemyPrefab => m_EnemyComponent.ValueRO.EnemyPrefab;

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
        float3 randomPosition;
        ref EnemySpawnerSingleton enemyComponent = ref this.m_EnemyComponent.ValueRW;

        randomPosition = enemyComponent.Randomizer.NextFloat3(m_MinCorner, m_MaxCorner);

        return randomPosition;
    }

    private float3 m_MinCorner => m_Local.ValueRO.Position - m_HalfDimensions;
    private float3 m_MaxCorner => m_Local.ValueRO.Position + m_HalfDimensions;

    private float3 m_HalfDimensions => new()
    {
        x = m_EnemyComponent.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = m_EnemyComponent.ValueRO.FieldDimensions.y * 0.5f,
    };
}
