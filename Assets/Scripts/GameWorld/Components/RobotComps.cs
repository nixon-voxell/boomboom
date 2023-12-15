using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

public class RobotBase : IComponentData
{
    public Material Material;
}

public struct RobotEyes : IComponentData
{
    public float4 Color;
    public float4 OriginColor;

    public RobotEye LeftEye;
    public RobotEye OriginLeftEye;

    public RobotEye RightEye;
    public RobotEye OriginRightEye;

    public RobotEyes CopyCurrentDataToOrigin()
    {
        this.OriginColor = this.Color;
        this.OriginLeftEye = this.LeftEye;
        this.OriginRightEye = this.RightEye;

        return this;
    }
}

public struct RobotEyesTarget : IComponentData
{
    public float4 Color;
    public RobotEye LeftEye;
    public RobotEye RightEye;
}

public struct RobotEyeSpeed : IComponentData
{
    public float Value;
}

public struct RobotEye
{
    public float2 Center;
    public float2 Size;

    public static RobotEye Lerp(RobotEye a, RobotEye b, float t)
    {
        return new RobotEye
        {
            Center = math.lerp(a.Center, b.Center, t),
            Size = math.lerp(a.Size, b.Size, t),
        };
    }
}

public struct RobotEyeBlink : IComponentData
{
    public float IntervalMin;
    public float IntervalMax;

    public float TimeElapsed;
}

public struct RobotCleanup : ICleanupComponentData { }
