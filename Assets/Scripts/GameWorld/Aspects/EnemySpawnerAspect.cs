using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public readonly partial struct EnemySpawnerAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRO<LocalTransform> m_Local;
    private readonly RefRW<EnemySpawnerSingleton> m_EnemyComponent; //ecs component 

    // create new variables to assign ecs component values
    // RO means we can only read this value; RW means we will first get this value and can change it
    public int NumberEnemySpawn => m_EnemyComponent.ValueRO.NumberEnemySpawn;
    public Entity EnemyPrefab => m_EnemyComponent.ValueRO.EnemyPrefab;
    public float SpawnInterval => m_EnemyComponent.ValueRW.SpawnInterval;

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
        ref EnemySpawnerSingleton enemyComponent = ref this.m_EnemyComponent.ValueRW;   
        /* EXPLANATION
         * ref: means you take this ecs component directly instead of make a copy of it,
         * so, if you edit ref ecs component, means you are editing original ecs component.
         */

        float3 randomPosition = enemyComponent.Randomizer.NextFloat3(m_MinCorner, m_MaxCorner);

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
