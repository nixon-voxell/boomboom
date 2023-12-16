using Unity.Mathematics;
using Unity.Entities;

public struct Health : IComponentData
{
    public float Value;
}

public struct Damage : IComponentData
{
    public float Value;
}

public struct SecondaryVelocity : IComponentData
{
    public float3 Value;
}

public struct Speed : IComponentData
{
    public float Value;
}

public struct Dash : IComponentData
{
    public float Value;
    public float2 Direction;
}
