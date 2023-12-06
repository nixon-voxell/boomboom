using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;

public partial struct TnTBehavior : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        bool countExplode = Input.GetMouseButtonDown(0);

        if (countExplode == false)
        {
            return;
        }

        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (tntCountDown, entity) in
            SystemAPI.Query<RefRW<TnTCountDown>>().WithAll<LocalTransform>().WithEntityAccess()
        )
        {
            commands.DestroyEntity(entity);
        }

        commands.Playback(state.EntityManager);
    }
}


