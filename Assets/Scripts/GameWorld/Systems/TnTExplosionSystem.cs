using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using System.Diagnostics;

public partial struct TnTExplosionSystem : ISystem
{
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (
            var (tnt, transform, entity) in
            SystemAPI.Query<
                RefRW<TnT>, 
                RefRO<LocalTransform>
                >().WithEntityAccess()
        )
        { 
            if (tnt.ValueRO.CountDownTimer <=0)
            {
                commands.DestroyEntity(entity);
                NativeList<DistanceHit> distances = new NativeList<DistanceHit>(Allocator.Temp);
                physicsWorld.OverlapSphere(transform.ValueRO.Position, tnt.ValueRO.Radius, ref distances, CollisionFilter.Default);
                foreach (DistanceHit hit in distances)
                {
                    commands.SetEnabled(hit.Entity, false);
                }
            }
            tnt.ValueRW.CountDownTimer -= SystemAPI.Time.DeltaTime;
        }
        
        commands.Playback(state.EntityManager);
        
    }

  
}


