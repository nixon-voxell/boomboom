using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;

public partial struct RobotEyeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RobotBase>();
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
    }
}
