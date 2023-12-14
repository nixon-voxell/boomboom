using Unity.Mathematics;
using Unity.Entities;
using Unity.Rendering;

// RobotBase
[MaterialProperty("_EyeColor")]
public struct _EyeColor : IComponentData
{
    public float4 Value;
}

[MaterialProperty("_Eye0Center")]
public struct _Eye0Center : IComponentData
{
    public float2 Value;
}

[MaterialProperty("_Eye0Size")]
public struct _Eye0Size : IComponentData
{
    public float2 Value;
}

[MaterialProperty("_Eye1Center")]
public struct _Eye1Center : IComponentData
{
    public float2 Value;
}

[MaterialProperty("_Eye1Size")]
public struct _Eye1Size : IComponentData
{
    public float2 Value;
}

// RobotTip
[MaterialProperty("_TipColor")]
public struct _TipColor : IComponentData
{
    public float4 Value;
}

[MaterialProperty("_RotationAxis")]
public struct _RotationAxis : IComponentData
{
    public float3 Value;
}

[MaterialProperty("_Rotation")]
public struct _Rotation : IComponentData
{
    public float Value;
}
