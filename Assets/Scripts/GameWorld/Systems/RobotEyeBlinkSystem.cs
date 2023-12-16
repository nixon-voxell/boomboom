using Unity.Entities;

public partial struct RobotEyeBlinkSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RobotEyeBlink>();
        state.RequireForUpdate<RobotEyes>();
    }

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
