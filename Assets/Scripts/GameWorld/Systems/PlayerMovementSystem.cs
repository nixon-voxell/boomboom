using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;

public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserInputSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        foreach (
            var (velocity, damping, secondaryVelocity, speed, dash, transform) in
            SystemAPI.Query<
                RefRW<PhysicsVelocity>,
                RefRO<PhysicsDamping>,
                RefRW<SecondaryVelocity>,
                RefRO<Speed>,
                RefRW<Dash>,
                RefRO<LocalTransform>
            >().WithAll<Tag_Player>()
        )
        {
            float3 forward = transform.ValueRO.Forward();

            velocity.ValueRW.Linear = 0.0f;
            velocity.ValueRW.Angular = 0.0f;

            if (userInput.IsMoving)
            {
                // Overwrite velocity with the user's MoveAxis input
                velocity.ValueRW.Linear = new float3(userInput.MoveAxis.x, 0.0f, userInput.MoveAxis.y) * speed.ValueRO.Value;
            }

            float3 v = secondaryVelocity.ValueRW.Value;

            if (userInput.Dash)
            {
                // Calculate dash velocity
                v += forward * dash.ValueRO.Value;
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

public partial struct LookRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UserInputSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UserInputSingleton userInput = SystemAPI.GetSingleton<UserInputSingleton>();

        // Only update the direcion if the user is moving
        if (userInput.IsMoving == false)
        {
            return;
        }

        foreach (
            var (speed, transform) in
            SystemAPI.Query<RefRO<LookSpeed>, RefRW<LocalTransform>>()
        )
        {
            quaternion targetRotation = quaternion.LookRotation(
                new float3(userInput.MoveAxis.x, 0.0f, userInput.MoveAxis.y), math.up()
            );

            transform.ValueRW.Rotation = math.slerp(
                transform.ValueRO.Rotation, targetRotation,
                SystemAPI.Time.DeltaTime * speed.ValueRO.Value
            );
        }
    }
}
