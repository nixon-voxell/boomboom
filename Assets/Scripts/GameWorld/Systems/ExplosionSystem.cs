using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial struct ExplosionSystem : ISystem, ISystemStartStop
{
    public void OnStartRunning(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        Entity poolEntity = SystemAPI.GetSingletonEntity<ExplosionPoolSingleton>();
        Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(poolEntity);

        for (int e = 0; e < poolAspect.Entities.Length; e++)
        {
            Entity entity = poolAspect.Entities[e].Entity;

            commands.AddComponent<Disabled>(entity);
            commands.AddComponent<ExplosionForce>(entity);
            commands.AddComponent<ExplosionRadius>(entity);
            commands.AddComponent<ExplosionTimer>(entity);

            commands.AddComponent<Explode>(entity);
            commands.SetComponentEnabled<Explode>(entity, false);

            commands.AddComponent<Damage>(entity);
        }

        commands.Playback(state.EntityManager);
    }

    public void OnStopRunning(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (explosion, transform, entity) in
            SystemAPI.Query<ExplosionAspect, LocalTransform>()
            .WithAll<Explode>() // Explosion component must be true
            .WithEntityAccess()
        )
        {
            UnityEngine.Debug.Log("Explode");
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

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        // Perform onlt when player presses the bomb button.
        if (userInput.Bomb == false)
        {
            return;
        }

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        Entity poolEntity = SystemAPI.GetSingletonEntity<ExplosionPoolSingleton>();
        Pool.Aspect poolAspect = SystemAPI.GetAspect<Pool.Aspect>(poolEntity);

        foreach (
            LocalTransform transform in
            SystemAPI.Query<LocalTransform>().WithAll<Tag_Player>()
        )
        {
            Entity explosionEntity;
            Pool.GetNextEntity(ref poolAspect, out explosionEntity);

            UnityEngine.Debug.Log("Bomb!");

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

            // Set bomb timer
            RefRW<ExplosionTimer> timer = SystemAPI.GetComponentRW<ExplosionTimer>(explosionEntity);
            timer.ValueRW.Set(5.0f);
        }

        commands.Playback(state.EntityManager);
    }
}
