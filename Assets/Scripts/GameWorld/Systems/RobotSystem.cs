using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial struct RobotInitSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (children, eyes, entity) in
            SystemAPI.Query<DynamicBuffer<Child>, RefRO<RobotEyes>>()
            .WithEntityAccess()
            .WithAll<RobotInit>()
        )
        {
            foreach (Child child in children)
            {
                commands.AddComponent<_EyeColor>(child.Value, new _EyeColor { Value = eyes.ValueRO.OriginColor });

                commands.AddComponent<_Eye0Center>(child.Value, new _Eye0Center { Value = eyes.ValueRO.OriginLeftEye.Center });
                commands.AddComponent<_Eye0Size>(child.Value, new _Eye0Size { Value = eyes.ValueRO.OriginLeftEye.Size });

                commands.AddComponent<_Eye1Center>(child.Value, new _Eye1Center { Value = eyes.ValueRO.OriginRightEye.Center });
                commands.AddComponent<_Eye1Size>(child.Value, new _Eye1Size { Value = eyes.ValueRO.OriginLeftEye.Size });
            }

            commands.RemoveComponent<RobotInit>(entity);
        }

        commands.Playback(state.EntityManager);
    }
}

public partial struct RobotEyeBlinkSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RobotEyeBlink>();
        state.RequireForUpdate<RobotEyes>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            var (blink, eyes) in
            SystemAPI.Query<RefRW<RobotEyeBlink>, RefRW<RobotEyes>>()
        )
        {
            blink.ValueRW.TimeElapsed -= SystemAPI.Time.DeltaTime;

            if (blink.ValueRO.TimeElapsed > 0.0)
            {
                continue;
            }

            eyes.ValueRW.LeftEye.Size.y = 0.0f;
            eyes.ValueRW.RightEye.Size.y = 0.0f;

            blink.ValueRW.TimeElapsed = blink.ValueRW.Random.NextFloat(
                blink.ValueRO.IntervalMin, blink.ValueRO.IntervalMax
            );
        }
    }
}

public partial struct RobotEyeUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Update material
        foreach (
            var (children, eyes) in
            SystemAPI.Query<DynamicBuffer<Child>, RefRO<RobotEyes>>()
            .WithAbsent<RobotInit>()
        )
        {
            foreach (Child child in children)
            {
                RefRW<_EyeColor> eyeColor = SystemAPI.GetComponentRW<_EyeColor>(child.Value);
                RefRW<_Eye0Center> eye0Center = SystemAPI.GetComponentRW<_Eye0Center>(child.Value);
                RefRW<_Eye0Size> eye0Size = SystemAPI.GetComponentRW<_Eye0Size>(child.Value);
                RefRW<_Eye1Center> eye1Center = SystemAPI.GetComponentRW<_Eye1Center>(child.Value);
                RefRW<_Eye1Size> eye1Size = SystemAPI.GetComponentRW<_Eye1Size>(child.Value);

                eyeColor.ValueRW.Value = eyes.ValueRO.Color;

                eye0Center.ValueRW.Value = eyes.ValueRO.LeftEye.Center;
                eye0Size.ValueRW.Value = eyes.ValueRO.LeftEye.Size;

                eye1Center.ValueRW.Value = eyes.ValueRO.RightEye.Center;
                eye1Size.ValueRW.Value = eyes.ValueRO.RightEye.Size;
            }
        }

        // Update value to target value
        foreach (
            var (eyes, eyesTarget, speed) in
            SystemAPI.Query<RefRW<RobotEyes>, RefRO<RobotEyesTarget>, RefRO<RobotEyeSpeed>>()
        )
        {
            float lerpTime = SystemAPI.Time.DeltaTime * speed.ValueRO.Value;
            eyes.ValueRW.Color = math.lerp(eyes.ValueRO.Color, eyesTarget.ValueRO.Color, lerpTime);
            eyes.ValueRW.LeftEye = RobotEye.Lerp(eyes.ValueRO.LeftEye, eyesTarget.ValueRO.LeftEye, lerpTime);
            eyes.ValueRW.RightEye = RobotEye.Lerp(eyes.ValueRO.RightEye, eyesTarget.ValueRO.RightEye, lerpTime);
        }
    }
}
