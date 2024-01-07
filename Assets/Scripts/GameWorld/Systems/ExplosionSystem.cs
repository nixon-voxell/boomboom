using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct ExplosionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        Entity entity = commands.CreateEntity();
        commands.AddComponent<Explosion>(entity, new Explosion
        {
            ImpulseForce = 10.0f,
            Radius = 10.0f,
        });
        commands.AddComponent<LocalTransform>(entity, LocalTransform.Identity);

        commands.Playback(state.EntityManager);
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (explosion, transform, entity) in SystemAPI.Query<Explosion, LocalTransform>().WithEntityAccess())
        {
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.PhysicsWorld.CollisionWorld;

            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

            bool hasHits = physicsWorld.OverlapSphere(
                transform.Position, explosion.Radius, ref hits, CollisionFilter.Default
            );

            if (hasHits)
            {
                foreach (var hit in hits)
                {
                    if (SystemAPI.HasComponent<PhysicsVelocity>(hit.Entity))
                    {
                        LocalTransform hitTransform = SystemAPI.GetComponent<LocalTransform>(hit.Entity);
                        RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(hit.Entity);

                        float3 direction = math.normalize(hitTransform.Position - transform.Position);
                        float3 force = direction * explosion.ImpulseForce;

                        velocity.ValueRW.Linear += force;
                    }
                }
            }

            commands.SetEnabled(entity, false);
        }

        commands.Playback(state.EntityManager);
    }
}
