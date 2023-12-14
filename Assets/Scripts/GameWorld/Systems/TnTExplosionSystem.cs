using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct TnTExplosionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool countExplode = false;

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


