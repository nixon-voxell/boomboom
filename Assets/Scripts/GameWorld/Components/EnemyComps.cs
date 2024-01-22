using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public struct EnemySpawnerSingleton : IComponentData
{
    public float Radius;
    public Random Randomizer;
    public int PoolCount;
    public Entity Prefab;
}

public struct DisabledEnemy : IBufferElementData
{
    public Entity Entity;
}

public readonly partial struct EnemySpawnerAspect : IAspect
{
    public readonly RefRO<LocalTransform> Transform;
    public readonly RefRW<EnemySpawnerSingleton> Spawner;
    public readonly DynamicBuffer<DisabledEnemy> DisabledEnemies;

    public ref EnemySpawnerSingleton SpawnerRW => ref this.Spawner.ValueRW;

    public void Spawn(ref EntityCommandBuffer commands, float height = 0.0f)
    {
        // Not spawning because there are no disabled enemies left to spawn.
        if (this.DisabledEnemies.Length == 0)
        {
            return;
        }

        // Spawn enemy in a random position (based on a radius) looking towards the center.
        float3 position = this.GetRandomPosition();
        quaternion rotation = quaternion.LookRotation(math.normalize(0.0f - position), math.up());
        position.y += height;

        Entity entity = this.GetNextEntity();

        commands.SetEnabled(entity, true);
        commands.SetComponent<LocalTransform>(
            entity, new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = 1.0f,
            }
        );
    }

    private float3 GetRandomPosition()
    {
        float2 direction = this.SpawnerRW.Randomizer.NextFloat2Direction();
        float2 position = direction * this.SpawnerRW.Radius;
        return new float3(position.x, 0.0f, position.y) + this.Transform.ValueRO.Position;
    }

    private Entity GetNextEntity()
    {
        Entity entity = this.DisabledEnemies[0].Entity;
        this.DisabledEnemies.RemoveAtSwapBack(0);

        return entity;
    }
}

[InternalBufferCapacity(8)]
public struct EnemyFragmentPool : IBufferElementData
{
    public Entity PoolEntity;
    public Entity FragmentPrefab;
    public int PoolCount;
}

public struct EnemyProgressionSingleton : IComponentData
{
    public float SpawnRate;
    public float Speed;
}
