using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

public partial struct EnemySetupSystem : ISystem, ISystemStartStop
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

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct EnemyDamageSystem : ISystem
{
    [BurstCompile]
    public partial struct EnemyDamageEvents : ICollisionEventsJob
    {
        public float EnemyDamage;
        public float EnemyForce;

        public NativeReference<float> TotalDamage;
        public NativeReference<float2> TotalForce;

        [ReadOnly] public Entity PlayerEntity;
        [ReadOnly] public ComponentLookup<Tag_Enemy> EnemyLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        public ComponentLookup<EnemyCanAttack> EnemyAttackLookup;
        public ComponentLookup<Timer> TimerLookup;

        public void Execute(CollisionEvent evt)
        {
            Entity playerEntity = Entity.Null;
            Entity enemyEntity = Entity.Null;

            // Find player and enemy entity
            if (evt.EntityA == this.PlayerEntity)
            {
                playerEntity = evt.EntityA;
            }
            else if (evt.EntityB == this.PlayerEntity)
            {
                playerEntity = evt.EntityB;
            }

            if (this.EnemyLookup.HasComponent(evt.EntityA))
            {
                enemyEntity = evt.EntityA;
            }
            else if (this.EnemyLookup.HasComponent(evt.EntityB))
            {
                enemyEntity = evt.EntityB;
            }

            if (playerEntity == Entity.Null || enemyEntity == Entity.Null)
            {
                return;
            }

            if (this.EnemyAttackLookup.IsComponentEnabled(enemyEntity) == false)
            {
                return;
            }

            // Perform attack
            ref readonly LocalTransform playerTransform = ref TransformLookup.GetRefRO(playerEntity).ValueRO;
            ref readonly LocalTransform enemyTransform = ref TransformLookup.GetRefRO(enemyEntity).ValueRO;
            ref Timer timer = ref TimerLookup.GetRefRW(enemyEntity).ValueRW;

            float2 direction = math.normalize((playerTransform.Position - enemyTransform.Position).xz);

            this.TotalForce.Value += direction * this.EnemyForce;
            this.TotalDamage.Value += this.EnemyDamage;

            // Reset attack
            timer.Reset();
            this.EnemyAttackLookup.SetComponentEnabled(enemyEntity, false);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyProgressionSingleton>();
        state.RequireForUpdate<Tag_PlayerSingleton>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EnemyProgressionSingleton progression = SystemAPI.GetSingleton<EnemyProgressionSingleton>();

        Entity playerEntity = SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>();

        ComponentLookup<Tag_Enemy> enemyLookup = SystemAPI.GetComponentLookup<Tag_Enemy>(true);
        ComponentLookup<LocalTransform> transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        ComponentLookup<EnemyCanAttack> enemyAttackLookup = SystemAPI.GetComponentLookup<EnemyCanAttack>();
        ComponentLookup<Timer> timerLookup = SystemAPI.GetComponentLookup<Timer>();

        NativeReference<float> totalDamage = new NativeReference<float>(0.0f, Allocator.TempJob);
        NativeReference<float2> totalForce = new NativeReference<float2>(0.0f, Allocator.TempJob);

        state.Dependency = new EnemyDamageEvents
        {
            EnemyDamage = progression.CurrDamage,
            EnemyForce = progression.Force,

            TotalDamage = totalDamage,
            TotalForce = totalForce,

            PlayerEntity = playerEntity,
            EnemyLookup = enemyLookup,
            TransformLookup = transformLookup,
            EnemyAttackLookup = enemyAttackLookup,
            TimerLookup = timerLookup,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        ref PhysicsVelocity velocity = ref SystemAPI.GetComponentRW<PhysicsVelocity>(playerEntity).ValueRW;
        ref Health health = ref SystemAPI.GetComponentRW<Health>(playerEntity).ValueRW;

        state.CompleteDependency();

        // Knockback
        velocity.Linear.xz += totalForce.Value;
        // Damage player health
        health.Value = math.max(health.Value - totalDamage.Value, 0.0f);

        if (totalDamage.Value > 0.0f)
        {
            // Perform damage effect
            PostProcessEffect.Instance.ChromaticIntensity = 1.0f;
            PostProcessEffect.Instance.DistortIntensity = 0.6f;

            // Play damage audio
            AudioManager.Instance.PlaySfx("Glitch");
        }

        totalDamage.Dispose();
        totalForce.Dispose();
    }
}

public partial struct EnemyAttackCooldownSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (timer, entity) in
            SystemAPI.Query<RefRW<Timer>>()
            .WithAll<Tag_Enemy>()
            .WithDisabled<EnemyCanAttack>()
            .WithEntityAccess()
        )
        {
            if (timer.ValueRW.Update(SystemAPI.Time.DeltaTime))
            {
                SystemAPI.SetComponentEnabled<EnemyCanAttack>(entity, true);
            }
        }

        commands.Playback(state.EntityManager);
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
        t = math.clamp(t, 0.0f, 1.0f);

        progression.CurrSpeed = math.lerp(progression.StartSpeed, progression.EndSpeed, t);
        progression.CurrSpawnRate = math.lerp(progression.StartSpawnRate, progression.EndSpawnRate, t);

        // Set timer to current spawn rate
        ref Timer timer = ref SystemAPI.GetComponentRW<Timer>(entity).ValueRW;
        timer.TotalTime = progression.CurrSpawnRate;
    }
}
