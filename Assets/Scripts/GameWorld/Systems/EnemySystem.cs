using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial struct EnemySetupSystem : ISystem, ISystemStartStop //for onstartrunning & stoprunning
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnerSingleton>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        Entity entity = SystemAPI.GetSingletonEntity<EnemySpawnerAspect>();
        EnemySpawnerAspect spawnerAspect = SystemAPI.GetAspect<EnemySpawnerAspect>(entity);

        // Provide a different seed everytime on every startup
        spawnerAspect.SpawnerRW.Randomizer = Random.CreateFromIndex((uint)System.DateTime.Now.Millisecond);

        // Instantiate all the enemy prefab and fill in the DisabledEnemies buffer
        for (int e = 0; e < spawnerAspect.SpawnerRW.PoolCount; e++)
        {
            spawnerAspect.DisabledEnemies.Add(
                new DisabledEnemy
                {
                    Entity = state.EntityManager.Instantiate(spawnerAspect.SpawnerRW.Prefab),
                }
            );
        }
    }

    public void OnStopRunning(ref SystemState state) { }
}

public partial struct EnemyFragmentSetupSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tag_EnemyFragmentsSingleton>();
    }

    public void OnStartRunning(ref SystemState state)
    {
    }

    public void OnStopRunning(ref SystemState state) { }
}

[UpdateAfter(typeof(EnemySetupSystem)), UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate<EnemySpawnerSingleton>();
        state.RequireForUpdate<EnemySpawnerSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity entity = SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>();

        // Get spawner aspect
        EnemySpawnerAspect spawnerAspect = SystemAPI.GetAspect<EnemySpawnerAspect>(entity);
        // Get timer
        RefRW<Timer> timer = SystemAPI.GetComponentRW<Timer>(entity);

        if (timer.ValueRW.Update(SystemAPI.Time.DeltaTime) == false)
        {
            return;
        }

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Reset timer
        timer.ValueRW.Reset();

        // Spawn enemies
        spawnerAspect.Spawn(ref commands, 10.0f);

        commands.Playback(state.EntityManager);
    }
}

public partial struct EnemyFollowSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tag_PlayerSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(
            SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>()
        );

        foreach (
            var (transform, velocity) in
            SystemAPI.Query<RefRO<LocalTransform>, RefRW<PhysicsVelocity>>()
            .WithAll<Tag_Enemy>()
        )
        {
            float3 v = velocity.ValueRO.Linear;
            float3 direction = math.normalizesafe(playerTransform.Position - transform.ValueRO.Position);

            float followSpeed = 8.0f;
            float3 followVelocity = direction * followSpeed;

            v += followVelocity * SystemAPI.Time.DeltaTime;
            velocity.ValueRW.Linear = v;
        }
    }
}
