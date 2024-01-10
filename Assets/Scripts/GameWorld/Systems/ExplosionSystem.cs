using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial struct ExplosionSetupSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionPoolSingleton>();
        state.RequireForUpdate<LandminePoolSingleton>();
        state.RequireForUpdate<ExplosionVfxPoolSingleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        // Add components to explosion pool
        Pool.Aspect explosionAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<ExplosionPoolSingleton>()
        );

        for (int e = 0; e < explosionAspect.Entities.Length; e++)
        {
            Entity entity = explosionAspect.Entities[e].Entity;

            commands.AddComponent<ExplosionForce>(entity);
            commands.AddComponent<ExplosionRadius>(entity);
            commands.AddComponent<ExplosionTimer>(entity);

            commands.AddComponent<Explode>(entity);
            commands.SetComponentEnabled<Explode>(entity, false);

            // TODO: Remove this?
            commands.AddComponent<Damage>(entity);

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

[UpdateBefore(typeof(ExplosionForceSystem))]
public partial struct ExplosionVfxSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionVfxPoolSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Pool.Aspect vfxAspect = SystemAPI.GetAspect<Pool.Aspect>(
            SystemAPI.GetSingletonEntity<ExplosionVfxPoolSingleton>()
        );

        foreach (
            LocalTransform transform in
            SystemAPI.Query<LocalTransform>()
            .WithAll<Explode>()
        )
        {
        }
    }
}

public partial struct ExplosionForceSystem : ISystem
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
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

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
                        LocalTransform hitTransform = SystemAPI.GetComponent<LocalTransform>(hit.Entity);
                        RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(hit.Entity);

                        float3 direction = math.normalizesafe(hitTransform.Position - transform.Position);
                        float3 angularDirection = -math.normalizesafe(math.cross(direction, math.up()));

                        velocity.ValueRW.Linear += direction * explosion.ForceRW;
                        velocity.ValueRW.Angular += angularDirection * explosion.ForceRW;
                    }
                }
            }

            commands.SetEnabled(entity, false);

            // Disable landmine
            commands.SetEnabled(explode.LandmineEntity, false);
        }

        commands.Playback(state.EntityManager);
    }
}

public partial struct ExplosionTimerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            var (timer, entity) in SystemAPI.Query<RefRW<ExplosionTimer>>()
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

public partial struct ExplosionPlacementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExplosionPoolSingleton>();
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

        Entity explosionPoolEntity = SystemAPI.GetSingletonEntity<ExplosionPoolSingleton>();
        Entity landminePoolEntity = SystemAPI.GetSingletonEntity<LandminePoolSingleton>();

        Pool.Aspect explosionAspect = SystemAPI.GetAspect<Pool.Aspect>(explosionPoolEntity);
        Pool.Aspect landmineAspect = SystemAPI.GetAspect<Pool.Aspect>(landminePoolEntity);

        foreach (
            LocalTransform transform in
            SystemAPI.Query<LocalTransform>().WithAll<Tag_Player>()
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
            SystemAPI.SetComponent<ExplosionForce>(explosionEntity, new ExplosionForce
            {
                Value = 10.0f,
            });
            SystemAPI.SetComponent<ExplosionRadius>(explosionEntity, new ExplosionRadius
            {
                Value = 2.0f,
            });
            SystemAPI.SetComponent<Explode>(explosionEntity, new Explode
            {
                LandmineEntity = landmineEntity,
            });
            SystemAPI.SetComponentEnabled<Explode>(explosionEntity, false);

            // Set bomb timer
            RefRW<ExplosionTimer> timer = SystemAPI.GetComponentRW<ExplosionTimer>(explosionEntity);
            timer.ValueRW.Set(5.0f);
        }

        commands.Playback(state.EntityManager);
    }
}
