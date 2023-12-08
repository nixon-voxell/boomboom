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
        EntityQueryBuilder queryBuilder =
            new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Tag_Player, PhysicsDamping, Speed>()
                .WithAllRW<PhysicsVelocity>()
                .WithAllRW<SecondaryVelocity, Dash>();

        state.RequireForUpdate(queryBuilder.Build(ref state));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        foreach (
            var (velocity, damping, secondaryVelocity, speed, dash) in
            SystemAPI.Query<
                RefRW<PhysicsVelocity>,
                RefRO<PhysicsDamping>,
                RefRW<SecondaryVelocity>,
                RefRO<Speed>,
                RefRW<Dash>
            >().WithAll<Tag_Player>()
        )
        {
            // Overwrite velocity with the user's MoveAxis input
            velocity.ValueRW.Linear = new float3(
                userInput.MoveAxis.x, 0.0f, userInput.MoveAxis.y
            ) * speed.ValueRO.Value;

            float3 v = secondaryVelocity.ValueRW.Value;

            // Only update the direcion if the user is moving
            if (math.lengthsq(userInput.MoveAxis) > 0.0f)
            {
                dash.ValueRW.Direction = userInput.MoveAxis;
            }

            if (userInput.Dash)
            {
                // Calculate dash velocity
                v += new float3(
                    dash.ValueRO.Direction.x, 0.0f, dash.ValueRO.Direction.y
                ) * dash.ValueRO.Value;
            }

            // TODO: Move this computation out of this system (it can be independant)
            // Manually damp secondary velocity
            v -= v * math.min(1.0f, damping.ValueRO.Linear * SystemAPI.Time.DeltaTime);

            // Update velocity value for the next frame
            secondaryVelocity.ValueRW.Value = v;
            // Add the resultant secondary velocity to the physics linear velocity
            velocity.ValueRW.Linear += v;
        }
    }
}
