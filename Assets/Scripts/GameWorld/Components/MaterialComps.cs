using Unity.Mathematics;
using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Position")]
public struct _Position : IComponentData
{
    public float3 Value;
}

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

[MaterialProperty("_Eye1Center")]
public struct _Eye1Center : IComponentData
{
    public float2 Value;
}

[MaterialProperty("_Eye0Size")]
public struct _Eye0Size : IComponentData
{
    public float2 Value;
}

[MaterialProperty("_Eye1Size")]
public struct _Eye1Size : IComponentData
{
    public float2 Value;
}
