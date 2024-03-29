using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial class ExplosionPreparationSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(ExplosionPreparationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class ExplosionProgressSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(ExplosionPreparationSystemGroup))]
public partial class ExplosionCompleteSystemGroup : ComponentSystemGroup { }

// =============================================
// Preparation
// =============================================

[UpdateInGroup(typeof(ExplosionPreparationSystemGroup))]
public partial struct ExplosionSetupSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionSingleton>();
        state.RequireForUpdate<LandminePoolSingleton>();
        state.RequireForUpdate<ExplosionVfxPoolSingleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Add components to explosion pool
        Pool.Aspect explosionAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<ExplosionSingleton>()
        );

        for (int e = 0; e < explosionAspect.Entities.Length; e++)
        {
            Entity entity = explosionAspect.Entities[e].Entity;

            commands.SetComponentEnabled<Explode>(entity, false);
            commands.SetEnabled(entity, false);
        }

        commands.Playback(state.EntityManager);

        // Instantiate landmine pool
        LandminePoolSingleton landmineSingleton = SystemAPI.GetSingleton<LandminePoolSingleton>();
        Pool.Aspect landmineAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<LandminePoolSingleton>()
        );

        EntityManager manager = state.EntityManager;
        Pool.InstantiatePrefabs(
            ref manager,
            ref landmineAspect,
            in landmineSingleton.Prefab,
            in landmineSingleton.PoolCount
        );

        // Instantiate explosion vfx pool
        ExplosionVfxPoolSingleton explosionVfxSingleton = SystemAPI.GetSingleton<ExplosionVfxPoolSingleton>();
        Pool.Aspect explosionVfxAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<ExplosionVfxPoolSingleton>()
        );

        Pool.InstantiatePrefabs(
            ref manager,
            ref explosionVfxAspect,
            in explosionVfxSingleton.Prefab,
            in explosionVfxSingleton.PoolCount
        );
    }

    public void OnStopRunning(ref SystemState state) { }
}

[UpdateInGroup(typeof(ExplosionPreparationSystemGroup))]
public partial struct ExplosionTimerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            var (timer, entity) in SystemAPI.Query<RefRW<Timer>>()
            .WithDisabled<Explode>()
            .WithEntityAccess()
        )
        {
            if (timer.ValueRW.Update(SystemAPI.Time.DeltaTime))
            {
                // Perform explosion
                SystemAPI.SetComponentEnabled<Explode>(entity, true);
            }
        }
    }
}

[UpdateInGroup(typeof(ExplosionPreparationSystemGroup))]
public partial struct ExplosionPlacementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserInputSingleton>();
        state.RequireForUpdate<ExplosionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        // Perform only when player presses the bomb button.
        if (userInput.Bomb == false)
        {
            return;
        }

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        Entity explosionPoolEntity = SystemAPI.GetSingletonEntity<ExplosionSingleton>();
        Entity landminePoolEntity = SystemAPI.GetSingletonEntity<LandminePoolSingleton>();

        Pool.Aspect explosionAspect = SystemAPI.GetAspect<Pool.Aspect>(explosionPoolEntity);
        Pool.Aspect landmineAspect = SystemAPI.GetAspect<Pool.Aspect>(landminePoolEntity);

        foreach (
            LocalTransform transform in
            SystemAPI.Query<LocalTransform>().WithAll<Tag_PlayerSingleton>()
        )
        {
            // Create a landmine at the player's position
            Entity landmineEntity;
            Pool.GetNextEntity(ref landmineAspect, out landmineEntity);

            SystemAPI.SetComponent<LocalTransform>(landmineEntity, transform.WithPosition(transform.Position + math.down() * 0.5f));
            commands.SetEnabled(landmineEntity, true);

            // Create an explosion at the player's position
            Entity explosionEntity;
            Pool.GetNextEntity(ref explosionAspect, out explosionEntity);

            commands.SetEnabled(explosionEntity, true);
            // Set bomb transform
            SystemAPI.SetComponent<LocalTransform>(explosionEntity, transform);
            // Set landmine reference
            SystemAPI.SetComponent<Explode>(explosionEntity, new Explode
            {
                LandmineEntity = landmineEntity,
            });
            SystemAPI.SetComponentEnabled<Explode>(explosionEntity, false);

            // Reset bomb timer
            RefRW<Timer> timer = SystemAPI.GetComponentRW<Timer>(explosionEntity);
            timer.ValueRW.Reset();
        }

        commands.Playback(state.EntityManager);
    }
}

// =============================================
// Progress
// =============================================

[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionVfxSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionVfxPoolSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Pool.Aspect vfxAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<ExplosionVfxPoolSingleton>()
        );

        EntityManager manager = state.EntityManager;
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            RefRO<LocalTransform> transform in
            SystemAPI.Query<RefRO<LocalTransform>>()
            .WithAll<Explode>()
        )
        {
            // Perform explosion at the given position
            Entity vfxEntity;
            Pool.GetNextEntity(ref vfxAspect, out vfxEntity);

            SystemAPI.SetComponent<LocalTransform>(vfxEntity, transform.ValueRO);
            commands.SetEnabled(vfxEntity, true);
            SystemAPI.GetComponentRW<Timer>(vfxEntity).ValueRW.Reset();
        }

        commands.Playback(manager);
    }
}

[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionVfxTimerFunction : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionVfxPoolSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityManager manager = state.EntityManager;
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (timer, vfx, vfxEntity) in
            SystemAPI.Query<RefRW<Timer>, RefRO<ExplosionVfx>>()
            .WithEntityAccess()
        )
        {
            UnityEngine.Light light = manager.GetComponentObject<UnityEngine.Light>(vfxEntity);

            if (timer.ValueRW.Update(SystemAPI.Time.DeltaTime))
            {
                commands.SetEnabled(vfxEntity, false);
            }

            light.intensity = math.lerp(
                vfx.ValueRO.LightBrightness, 0.0f,
                timer.ValueRO.ElapsedTime / (timer.ValueRO.TotalTime * 0.5f)
            );
            // Prevent from going below 0
            light.intensity = math.max(light.intensity, 0.0f);
        }

        commands.Playback(manager);
    }
}

[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionCamShakeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ExplosionSingleton explosionSingleton = SystemAPI.GetSingleton<ExplosionSingleton>();

        foreach (
            RefRO<Explode> _ in
            SystemAPI.Query<RefRO<Explode>>()
        )
        {
            VirtualCameraMono.Instance.AddNoiseAmplitude(explosionSingleton.ShakeAmplitude);
            VirtualCameraMono.Instance.AddNoiseFrequency(explosionSingleton.ShakeFrequency);
        }
    }
}

/// <summary>Exert force on surrounding physics bodies and kill enemies within it.</summary>
[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionForceSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyFragmentPool>();
        state.RequireForUpdate<EnemySpawnerSingleton>();
        state.RequireForUpdate<GameStatSingleton>();
        state.RequireForUpdate<Tag_PlayerSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Get game stats
        ref GameStatSingleton gameStat = ref SystemAPI.GetSingletonRW<GameStatSingleton>().ValueRW;

        // Physics world for collision
        PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

        // Enemy fragment pool
        DynamicBuffer<EnemyFragmentPool> fragmentPools = SystemAPI.GetSingletonBuffer<EnemyFragmentPool>();

        // Disabled enemy buffer
        DynamicBuffer<DisabledEnemy> disabledEnemies = SystemAPI.GetBuffer<DisabledEnemy>(
            SystemAPI.GetSingletonEntity<EnemySpawnerSingleton>()
        );

        // Player's health
        ref Health health = ref SystemAPI.GetComponentRW<Health>(SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>()).ValueRW;

        foreach (
            var (explosion, explode, transform, entity) in
            SystemAPI.Query<ExplosionAspect, Explode, LocalTransform>()
            .WithEntityAccess()
        )
        {
            hits.Clear();
            bool hasHits = physicsWorld.OverlapSphere(
                transform.Position,
                explosion.Radius.ValueRO.Value,
                ref hits,
                CollisionFilter.Default
            );

            ref float explosionForce = ref explosion.ForceRW;
            float3 explosionPosition = transform.Position;

            if (hasHits)
            {
                foreach (var hit in hits)
                {
                    Entity hitEntity = hit.Entity;

                    // Perform explosion on enemies
                    if (SystemAPI.HasComponent<PhysicsVelocity>(hitEntity))
                    {
                        if (SystemAPI.HasComponent<Tag_Enemy>(hitEntity))
                        {
                            ref readonly LocalTransform enemyTransform =
                                ref SystemAPI.GetComponentRO<LocalTransform>(hitEntity).ValueRO;

                            // Destroy enemies
                            GameUtil.KillEnemy(ref commands, ref disabledEnemies, in hitEntity);
                            gameStat.KillCount += 1;

                            // Each kill heals 1 health for the player
                            health.Value = math.min(health.Value + 1, 100.0f);

                            for (int p = 0; p < fragmentPools.Length; p++)
                            {
                                Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(fragmentPools[p].PoolEntity);

                                Entity fragmentEntity;
                                Pool.GetNextEntity(ref poolAspect, out fragmentEntity);

                                SystemAPI.SetComponent<LocalTransform>(fragmentEntity, enemyTransform);
                                commands.SetEnabled(fragmentEntity, true);

                                PerformExplosionForce(
                                    ref state,
                                    explosionForce,
                                    explosionPosition,
                                    fragmentEntity,
                                    hit.Position
                                );
                            }
                        }
                        else
                        {
                            PerformExplosionForce(
                                ref state,
                                explosionForce,
                                explosionPosition,
                                hitEntity,
                                hit.Position
                            );
                        }
                    }
                }
            }
        }

        commands.Playback(state.EntityManager);
    }

    private void PerformExplosionForce(
        ref SystemState state,
        float explosionForce,
        float3 explosionPosition,
        Entity hitEntity,
        float3 hitPosition
    )
    {
        ref PhysicsVelocity velocity = ref SystemAPI.GetComponentRW<PhysicsVelocity>(hitEntity).ValueRW;
        ref readonly PhysicsMass mass = ref SystemAPI.GetComponentRO<PhysicsMass>(hitEntity).ValueRO;

        float3 direction = math.normalizesafe(hitPosition - explosionPosition);
        float3 angularDirection = -math.normalizesafe(math.cross(direction, math.up()));

        float force = explosionForce * mass.InverseMass;
        velocity.Linear += direction * force;
        velocity.Angular += angularDirection * force * 1.5f;
    }
}

// =============================================
// Complete
// =============================================

[UpdateInGroup(typeof(ExplosionCompleteSystemGroup))]
public partial struct ExplosionCompleteSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (explosion, explode, entity) in
            SystemAPI.Query<ExplosionAspect, Explode>()
            .WithEntityAccess()
        )
        {
            commands.SetEnabled(entity, false);

            // Disable landmine
            commands.SetEnabled(explode.LandmineEntity, false);
        }

        commands.Playback(state.EntityManager);
    }
}
