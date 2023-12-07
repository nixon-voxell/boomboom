using Unity.Collections;
using Unity.Entities;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserInputSingleton>();

        // Player with Speed component attached to it
        EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<Tag_Player, Speed>();

        state.RequireForUpdate(queryBuilder.Build(ref state));
    }
}
