using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

public partial struct RobotEyeSystem : ISystem, ISystemStartStop
{
    public void OnStartRunning(ref SystemState state)
    {
        EntityCommandBuffer commands = new EntityCommandBuffer(Allocator.Temp);

        foreach (
            var (robotBase, entity) in
            SystemAPI.Query<RobotBase>().WithEntityAccess()
        )
        {
            Material material = robotBase.Material;

            // Create RobotEyes based on material data
            RobotEyes eyes = new RobotEyes
            {
                Color = ManagedUtil.ColorToFloat4(material.GetColor(ShaderID._EyeColor)),
                LeftEye = new RobotEye
                {
                    Center = ManagedUtil.Vector4ToFloat2(material.GetVector(ShaderID._Eye0Center)),
                    Size = ManagedUtil.Vector4ToFloat2(material.GetVector(ShaderID._Eye0Size)),
                },
                RightEye = new RobotEye
                {
                    Center = ManagedUtil.Vector4ToFloat2(material.GetVector(ShaderID._Eye1Center)),
                    Size = ManagedUtil.Vector4ToFloat2(material.GetVector(ShaderID._Eye1Size)),
                }
            }.CopyCurrentDataToOrigin();

            commands.AddComponent<RobotEyes>(entity, eyes);
            commands.AddComponent<RobotEyesTarget>(entity, new RobotEyesTarget
            {
                Color = eyes.Color,
                LeftEye = eyes.LeftEye,
                RightEye = eyes.RightEye,
            });

            commands.AddComponent<RobotCleanup>(entity);
        }

        commands.Playback(state.EntityManager);
    }

    public void OnUpdate(ref SystemState state)
    {
        // Update material
        foreach (
            var (robotBase, eyes) in
            SystemAPI.Query<RobotBase, RefRO<RobotEyes>>()
        )
        {
            Material material = robotBase.Material;

            material.SetVector(ShaderID._EyeColor, eyes.ValueRO.Color);

            material.SetVector(ShaderID._Eye0Center, (Vector2)eyes.ValueRO.LeftEye.Center);
            material.SetVector(ShaderID._Eye0Size, (Vector2)eyes.ValueRO.LeftEye.Size);

            material.SetVector(ShaderID._Eye1Center, (Vector2)eyes.ValueRO.RightEye.Center);
            material.SetVector(ShaderID._Eye1Size, (Vector2)eyes.ValueRO.RightEye.Size);
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

        // Blinking
    }

    public void OnStopRunning(ref SystemState state)
    {
        foreach (
            var (robotBase, eyes) in
            SystemAPI.Query<RobotBase, RefRO<RobotEyes>>()
        )
        {
            Material material = robotBase.Material;

            material.SetVector(ShaderID._EyeColor, eyes.ValueRO.OriginColor);

            material.SetVector(ShaderID._Eye0Center, (Vector2)eyes.ValueRO.OriginLeftEye.Center);
            material.SetVector(ShaderID._Eye0Size, (Vector2)eyes.ValueRO.OriginLeftEye.Size);

            material.SetVector(ShaderID._Eye1Center, (Vector2)eyes.ValueRO.OriginRightEye.Center);
            material.SetVector(ShaderID._Eye1Size, (Vector2)eyes.ValueRO.OriginRightEye.Size);
        }
    }
}
