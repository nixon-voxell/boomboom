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
        state.RequireForUpdate<EnemyFragmentPool>();
    }

    public void OnStartRunning(ref SystemState state)
    {
        DynamicBuffer<EnemyFragmentPool> fragmentPools = SystemAPI.GetSingletonBuffer<EnemyFragmentPool>();

        EntityManager manager = state.EntityManager;

        foreach (EnemyFragmentPool fragmentPool in fragmentPools)
        {
            DynamicBuffer<Pool.Element> poolBuffer = SystemAPI.GetBuffer<Pool.Element>(fragmentPool.PoolEntity);
            for (int p = 0; p < fragmentPool.PoolCount; p++)
            {
                poolBuffer.Add(new Pool.Element
                {
                    Entity = manager.Instantiate(fragmentPool.FragmentPrefab),
                });
            }
        }
    }

    public void OnStopRunning(ref SystemState state) { }
}

[UpdateAfter(typeof(EnemySetupSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tag_PlayerSingleton>();
        state.RequireForUpdate<EnemyProgressionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(
            SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>()
        );

        EnemyProgressionSingleton progression = SystemAPI.GetSingleton<EnemyProgressionSingleton>();

        foreach (
            var (transform, velocity) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>>()
            .WithAll<Tag_Enemy>()
        )
        {
            float3 v = velocity.ValueRO.Linear;
            float2 direction = math.normalizesafe(playerTransform.Position - transform.ValueRO.Position).xz;
            direction = math.normalizesafe(direction);

            v.xz = direction * progression.CurrSpeed;
            velocity.ValueRW.Linear = v;

            quaternion targetRotation = quaternion.LookRotation(
                new float3(direction.x, 0.0f, direction.y), math.up()
            );

            transform.ValueRW.Rotation = math.slerp(
                transform.ValueRO.Rotation, targetRotation,
                SystemAPI.Time.DeltaTime * progression.CurrSpeed
            );
        }
    }
}

public partial struct EnemyProgressionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameStatSingleton>();
        state.RequireForUpdate<EnemyProgressionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        GameStatSingleton gameStat = SystemAPI.GetSingleton<GameStatSingleton>();
        Entity entity = SystemAPI.GetSingletonEntity<EnemyProgressionSingleton>();
        ref EnemyProgressionSingleton progression = ref SystemAPI.GetComponentRW<EnemyProgressionSingleton>(entity).ValueRW;

        // Update progression based on survival time
        float t = gameStat.SurvivalTime / progression.EndTime;
        progression.CurrSpeed = math.lerp(progression.StartSpeed, progression.EndSpeed, t);
        progression.CurrSpawnRate = math.lerp(progression.StartSpawnRate, progression.EndSpawnRate, t);

        // Set timer to current spawn rate
        ref Timer timer = ref SystemAPI.GetComponentRW<Timer>(entity).ValueRW;
        timer.TotalTime = progression.CurrSpawnRate;
    }
}
