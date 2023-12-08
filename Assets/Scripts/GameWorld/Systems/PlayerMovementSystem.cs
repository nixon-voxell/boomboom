using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserInputSingleton>();

        // Player with Speed component attached to it
        EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<Tag_Player, Speed>();

        state.RequireForUpdate(queryBuilder.Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        foreach (
            var (velocity, speed) in
            SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<Speed>>().WithAll<Tag_Player>()
        )
        {
            velocity.ValueRW.Linear = new float3(
                userInput.MoveAxis.x, 0.0f, userInput.MoveAxis.y
            ) * speed.ValueRO.Value;
        }

        commands.Playback(state.EntityManager);
    }
}
