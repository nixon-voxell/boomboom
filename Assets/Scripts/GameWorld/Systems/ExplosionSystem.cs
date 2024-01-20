using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial class ExplosionPreparationSystemGroup : ComponentSystemGroup { }

[UpdateBefore(typeof(TransformSystemGroup)), UpdateAfter(typeof(ExplosionPreparationSystemGroup))]
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

            SystemAPI.SetComponent<LocalTransform>(landmineEntity, transform);
            commands.SetEnabled(landmineEntity, true);

            // Create an explosion at the player's position
            Entity explosionEntity;
            Pool.GetNextEntity(ref explosionAspect, out explosionEntity);

            commands.SetEnabled(explosionEntity, true);
            // Set bomb transform
            SystemAPI.SetComponent<LocalTransform>(explosionEntity, transform);
            // Set bomb explosion data
            // SystemAPI.SetComponent<ExplosionForce>(explosionEntity, new ExplosionForce
            // {
            //     Value = 10.0f,
            // });
            // SystemAPI.SetComponent<ExplosionRadius>(explosionEntity, new ExplosionRadius
            // {
            //     Value = 2.0f,
            // });
            SystemAPI.SetComponent<Explode>(explosionEntity, new Explode
            {
                LandmineEntity = landmineEntity,
            });
            SystemAPI.SetComponentEnabled<Explode>(explosionEntity, false);

            // Set bomb timer
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

[UpdateInGroup(typeof(ExplosionProgressSystemGroup))]
public partial struct ExplosionForceSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

        foreach (
            var (explosion, explode, transform, entity) in
            SystemAPI.Query<ExplosionAspect, Explode, LocalTransform>()
            .WithEntityAccess()
        )
        {
            hits.Clear();
            bool hasHits = physicsWorld.OverlapSphere(
                transform.Position, explosion.Radius.ValueRO.Value, ref hits, CollisionFilter.Default
            );

            if (hasHits)
            {
                foreach (var hit in hits)
                {
                    // Perform explosion on enemies
                    if (SystemAPI.HasComponent<PhysicsVelocity>(hit.Entity))
                    {
                        ref readonly LocalTransform hitTransform = ref SystemAPI.GetComponentRO<LocalTransform>(hit.Entity).ValueRO;
                        ref PhysicsVelocity velocity = ref SystemAPI.GetComponentRW<PhysicsVelocity>(hit.Entity).ValueRW;
                        ref readonly PhysicsMass mass = ref SystemAPI.GetComponentRO<PhysicsMass>(hit.Entity).ValueRO;

                        float3 direction = math.normalizesafe(hitTransform.Position - transform.Position);
                        float3 angularDirection = -math.normalizesafe(math.cross(direction, math.up()));

                        float force = explosion.ForceRW * mass.InverseMass;
                        velocity.Linear += direction * force;
                        velocity.Angular += angularDirection * force * 1.5f;
                    }
                }
            }
        }
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
            var (explosion, explode, transform, entity) in
            SystemAPI.Query<ExplosionAspect, Explode, LocalTransform>()
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
