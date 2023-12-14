using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public partial struct TnTExplosionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);
        

        foreach (
            var (tntCountDown, entity) in
            SystemAPI.Query<RefRW<TnTCountDown>>().WithAll<LocalTransform>().WithEntityAccess()
            
        )
        {
            if (tntCountDown.ValueRO.CountDownTimer <= 0)
            {
                commands.DestroyEntity(entity);
            }
            tntCountDown.ValueRW.CountDownTimer -= SystemAPI.Time.DeltaTime;

        }

        commands.Playback(state.EntityManager);
    }



}


