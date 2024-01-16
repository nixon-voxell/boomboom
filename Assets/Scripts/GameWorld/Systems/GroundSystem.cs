using Unity.Entities;
using Unity.Transforms;

public partial struct GroundSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Tag_GroundSingleton>();
        state.RequireForUpdate<Tag_PlayerSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        RefRW<_Position> groundMatPosition = SystemAPI.GetComponentRW<_Position>(
            SystemAPI.GetSingletonEntity<Tag_GroundSingleton>()
        );
        RefRO<LocalTransform> playerTransform = SystemAPI.GetComponentRO<LocalTransform>(
            SystemAPI.GetSingletonEntity<Tag_PlayerSingleton>()
        );

        groundMatPosition.ValueRW.Value = playerTransform.ValueRO.Position;
    }
}
